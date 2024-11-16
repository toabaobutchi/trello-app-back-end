using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Data;
using backend_apis.Exceptions;
using backend_apis.Models;
using backend_apis.Utils;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Repositories
{
    public class WorkspaceRepository : IWorkspaceRepository
    {
        private readonly ProjectManagerDbContext _db;

        public WorkspaceRepository(ProjectManagerDbContext db)
        {
            _db = db;
        }

        public async Task<Workspace?> CreateWorkspaceAsync(string ownerId, CreateWorkspaceModel model)
        {
            try
            {
                var workspace = model.ToWorkspace();
                workspace.OwnerId = ownerId;
                await _db.Workspaces.AddAsync(workspace);
                await _db.SaveChangesAsync();
                return workspace;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<WorkspaceResponse>> GetOwnWorkspacesAsync(string ownerId)
        {
            var userWorkspaces = await _db.Workspaces
                    .Where(w => w.OwnerId == ownerId)
                    .Select(w => WorkspaceResponse.Create(w, ContextResponse.Owner))
                    .AsNoTracking()
                    .ToListAsync();
            if (userWorkspaces == null) throw new EntityNotFoundException("Workspaces not found", EExceptionEntityType.Workspace);
            return userWorkspaces;
        }

        public async Task<List<Workspace?>?> GetSharedWorkspacesAsync(string userId)
        {
            try
            {
                var assignments = _db.Assignments
                                .Where(a => a.UserId == userId && a.Permission != EPermission.Owner)
                                .Include(a => a.Project)
                                    .ThenInclude(p => p.Workspace);
                var sharedWorkspaces = await assignments.Select(a => a.Project.Workspace).Distinct().ToListAsync();
                return sharedWorkspaces;
            }
            catch
            {
                return null;
            }
        }

        public async Task<WorkspaceResponseWithRelatedProjects?> GetSharedWorkspaceWithProjectsAsync(int workspaceId, string userId)
        {
            try
            {
                var workspace = await _db.Workspaces
                    .AsNoTracking()
                    .Include(w => w.Owner)
                    .Include(w => w.Projects)
                        .ThenInclude(p => p.Assignments)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(w => w.Id == workspaceId);
                if (workspace == null)
                {
                    return null;
                }
                workspace.Projects = workspace.Projects
                    .FindAll(p => p.Assignments.Any(a => a.Id == TextUtils.CreateAssignmentId(p.Id, userId)));
                var data = WorkspaceResponseWithRelatedProjects.Create(workspace, ContextResponse.Member, userId);
                data.Owner = OwnerInfo.Create(workspace.Owner);
                return data;
            }
            catch
            {
                return null;
            }
        }

        public async Task<WorkspaceResponseWithRelatedProjects?> GetWorkspaceWithProjectsAsync(int workspaceId, string userId)
        {
            try
            {
                var workspace = await _db.Workspaces
                    .AsNoTracking()
                    // .Include(w => w.Owner)
                    .Include(w => w.Projects)
                        .ThenInclude(p => p.Assignments)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(w => w.Id == workspaceId && w.OwnerId == userId);
                var data = WorkspaceResponseWithRelatedProjects.Create(workspace, ContextResponse.Owner, userId);
                data.Owner = null;
                return data;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Workspace?> UpdateWorkspacAsync(int workspaceId, string ownerId, WorkspaceUpdateModel model)
        {
            try
            {
                var workspace = await _db.Workspaces.FindAsync(workspaceId);
                if (workspace?.OwnerId != ownerId)
                {
                    Console.WriteLine(ownerId ?? "No id");
                    return null;
                }
                else
                {
                    bool needUpdating = false;
                    if (!string.IsNullOrEmpty(model.Name))
                    {
                        workspace.Name = model.Name;
                        workspace.Slug = TextUtils.CreateSlug(model.Name) + "-" + DateTimeUtils.GetSeconds();
                        needUpdating = true;
                    }
                    if (!string.IsNullOrEmpty(model.Description))
                    {
                        workspace.Description = model.Description;
                        needUpdating = true;
                    }
                    if (needUpdating)
                    {
                        await _db.SaveChangesAsync();
                    }
                    return workspace;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}