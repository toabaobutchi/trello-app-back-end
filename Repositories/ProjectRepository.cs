using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseForBoardDisplay;
using backend_apis.ApiModels.ResponseForBoardDisplay_v2;
using backend_apis.ApiModels.ResponseForBoardDisplay_v3;
using backend_apis.ApiModels.ResponseForTableDisplay;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Data;
using backend_apis.Exceptions;
using backend_apis.Models;
using backend_apis.Utils;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectManagerDbContext _db;
        public ProjectRepository(ProjectManagerDbContext db)
        {
            _db = db;
        }

        public async Task<Project?> CreateProjectAsync(CreateProjectModel model, string userId)
        {
            try
            {
                var project = model.ToProject();
                var assignment = new Assignment
                {
                    Id = TextUtils.CreateAssignmentId(project.Id, userId),
                    UserId = userId,
                    Permission = EPermission.Owner,
                    JoinAt = DateTimeUtils.GetSeconds()
                };
                project.Assignments.Add(assignment);
                await _db.Projects.AddAsync(project);
                await _db.SaveChangesAsync();
                return project;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<ProjectResponse>?> GetAllProjectsAsync(string userId)
        {
            try
            {
                var projects = await _db.Assignments
                    .Where(a => a.UserId == userId && a.Project.Assignments.Count > 1) // phải có ít nhất 1 người và không phải người request
                    .Include(a => a.Project)
                    .Select(a => ProjectResponse.Create(a.Project, a.Permission.ToString()))
                    .ToListAsync();
                return projects ?? [];
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<InvitedProjectResponse>?> GetInvitedProjectsAsync(string userId)
        {
            try
            {
                var user = await _db.Users.Select(u => new { u.Id, u.Email }).FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null) return null;
                var invitations = await _db.ProjectInvitations.Where(i => i.InvitedEmail == user.Email).Select(i => InvitedProjectResponse.Create(i, i.Project, i.Inviter.User.Email)).ToListAsync();
                return invitations ?? [];
            }
            catch
            {
                return null;
            }
        }

        public async Task<Project?> GetProjectAsync(string projectId)
        {
            try
            {
                var project = await _db.Projects.FindAsync(projectId);
                return project;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ProjectResponseForUpdating?> GetProjectForUpdatingAsync(string projectId)
        {
            try
            {
                var project = await _db.Projects.FindAsync(projectId);
                // tìm ngày due date tối thiểu mà project có thể cập nhật được
                var tasksInProject = _db.Tasks.Where(t => t.List.ProjectId == projectId && t.DueDate != null);
                var minDueDate = tasksInProject?.Max(t => t.DueDate);
                return ProjectResponseForUpdating.Create(project, minDueDate);
            }
            catch
            {
                return null;
            }
        }

        public async Task<ProjectResponseForBoardDisplay?> GetProjectResponseForBoardDisplayAsync(string projectId, string? permission = null, string? userId = null)
        {
            try
            {
                if (permission == null)
                {
                    var assignment = await _db.Assignments.FirstOrDefaultAsync(a => a.Id == TextUtils.CreateAssignmentId(projectId, userId));
                    if (assignment == null)
                    {
                        return null;
                    }
                    permission = assignment.Permission.ToString().ToLower();
                }
                var projectResponse = await (from project in _db.Projects
                                             where project.Id == projectId
                                             select ProjectResponseForBoardDisplay.Create(project, project.Assignments.Count, permission).SetLists(project.Lists)).AsNoTracking().FirstOrDefaultAsync();

                var tasksGroupByList = _db.Tasks.Where(t => t.List.ProjectId == projectId)
                                        .Include(t => t.TaskAssignments)
                                        .Include(t => t.SubTasks)
                                        .AsSplitQuery()
                                        .GroupBy(t => t.ListId)
                                        .AsNoTracking();

                foreach (var tasksGroup in tasksGroupByList)
                {
                    projectResponse?.Lists
                            .FirstOrDefault(l => l.Id == tasksGroup.Key)
                                ?.SetTasks(tasksGroup.Select(t => TaskResponseForBoardDisplay.Create(t)));
                }
                return projectResponse;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ProjectResponseForBoardDisplay_v2?> GetProjectResponseForBoardDisplayAsync_v2(string projectId, string? permission = null, string? userId = null)
        {
            try
            {
                if (permission == null)
                {
                    var assignment = await _db.Assignments.FindAsync(TextUtils.CreateAssignmentId(projectId, userId));
                    if (assignment == null)
                    {
                        return null;
                    }
                    permission = assignment.Permission.ToString().ToLower();
                }
                var projectResponse = await (from project in _db.Projects
                                             where project.Id == projectId
                                             select ProjectResponseForBoardDisplay_v2.Create(project, permission).SetLists(project.Lists)).AsNoTracking().FirstOrDefaultAsync();


                var tasksGroupByList = _db.Tasks.Where(t => t.List != null && t.List.ProjectId == projectId)
                                        .AsSplitQuery()
                                        .Include(t => t.TaskAssignments)
                                        .Include(t => t.SubTasks)
                                        .Include(t => t.Comments)
                                        .GroupBy(t => t.ListId)
                                        .AsNoTracking();

                foreach (var tasksGroup in tasksGroupByList)
                {
                    projectResponse?.Lists
                            .FirstOrDefault(l => l.Id == tasksGroup.Key)
                                ?.SetTasks(tasksGroup.Select(TaskResponseForBoardDisplay_v2.Create));
                }
                return projectResponse;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ProjectResponseForBoardDisplay_v3?> GetProjectResponseForBoardDisplayAsync_v3(string projectId, string? permission = null, string? userId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(permission))
                {
                    var assignment = await _db.Assignments.FindAsync(TextUtils.CreateAssignmentId(projectId, userId));
                    if (assignment == null)
                    {
                        return null;
                    }
                    permission = assignment.Permission.ToString().ToLower();
                }
                var projectResponse = await (from project in _db.Projects
                                             where project.Id == projectId
                                             select ProjectResponseForBoardDisplay_v3.Create(project, permission).SetLists(project.Lists)).AsNoTracking().FirstOrDefaultAsync();

                if (projectResponse == null)
                {
                    return null;
                }

                var tasksGroupByList = from task in _db.Tasks
                                       where task.List != null && task.List.ProjectId == projectId && (task.IsDeleted == null || task.IsDeleted == false)
                                       group task by task.ListId into taskGroup
                                       select new { taskGroup.Key, Elements = taskGroup.Select(tg => TaskResponseForBoardDisplay_v3.Create(tg, tg.TaskAssignments, tg.SubTasks, tg.Comments, tg.ParentDependentTasks)) };

                foreach (var tasksGroup in tasksGroupByList)
                {
                    projectResponse?.Lists
                            .FirstOrDefault(l => l.Id == tasksGroup.Key)
                                ?.SetTasks(tasksGroup.Elements);
                }
                return projectResponse;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ProjectResponseForTableDisplay?> GetProjectResponseForTableDisplayAsync(string projectId, string? permission = null, string? userId = null)
        {
            try
            {
                if (permission == null)
                {
                    var assignment = await _db.Assignments.FirstOrDefaultAsync(a => a.Id == TextUtils.CreateAssignmentId(projectId, userId));
                    if (assignment == null)
                    {
                        return null;
                    }
                    permission = assignment.Permission.ToString().ToLower();
                }
                var projectResponse = await (from project in _db.Projects
                                             where project.Id == projectId
                                             select ProjectResponseForTableDisplay.Create(project, permission)).AsNoTracking().FirstOrDefaultAsync();
                var tasksGroupByList = await _db.Tasks.Where(t => t.List.ProjectId == projectId)
                                        .AsSplitQuery()
                                        .AsNoTracking().Select(t => TaskResponseForTableDisplay.Create(t, t.List.Name, t.TaskAssignments)).ToListAsync();
                projectResponse.Tasks = tasksGroupByList;
                return projectResponse;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<InTrashTaskResponse>?> GetRecycleBinAsync(string projectId)
        {
            try
            {
                var inTrashTasks = await _db.Tasks
                    .Where(t => t.List != null && t.List.ProjectId == projectId && t.IsDeleted == true)
                    .Select(t => InTrashTaskResponse.Create(t, t.Deleter.User))
                    .ToListAsync();
                return inTrashTasks;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ProjectInvitation?> InviteAsync(string pid, string inviterId, InvitationModel model)
        {
            try
            {
                var isExitedInvitation = _db.ProjectInvitations.Any(pi => pi.InvitedEmail == model.Email && pi.ProjectId == pid);
                if (isExitedInvitation)
                {
                    throw new DuplicatedException("Invitation already exists");
                }
                else
                {
                    var invitation = model.ToProjectInvitation();
                    invitation.ProjectId = pid;
                    invitation.InviterId = TextUtils.CreateAssignmentId(pid, inviterId);
                    await _db.ProjectInvitations.AddAsync(invitation);
                    await _db.SaveChangesAsync();
                    return invitation;
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task<Project?> UpdateProjectAsync(string projectId, UpdateProjectModel model)
        {
            try
            {
                var project = await _db.Projects.FindAsync(projectId);
                if (project == null)
                {
                    return null;
                }
                if (model.DueDate != null)
                {
                    var tasksInProject = _db.Tasks.Where(t => t.List.ProjectId == projectId && t.DueDate != null);
                    var minDueDate = tasksInProject?.Max(t => t.DueDate);
                    if (minDueDate != null && model.DueDate != null && minDueDate > model.DueDate)
                    {
                        throw new OverDueDateException("Project due date is not valid");
                    }
                }
                project.Name = model.Name ?? project.Name;
                project.Description = model.Description;
                project.DueDate = model.DueDate;
                await _db.SaveChangesAsync();
                return project;
            }
            catch
            {
                return null;
            }
        }
    }
}