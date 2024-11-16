using System.Globalization;
using backend_apis.ApiModels;
using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Data;
using backend_apis.Hubs;
using backend_apis.Models;
using backend_apis.Utils;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly ProjectManagerDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IHubContext<ProjectHub> _hub;

        public AssignmentRepository(ProjectManagerDbContext db, IEmailSender emailSender, IHubContext<ProjectHub> hub)
        {
            _db = db;
            _emailSender = emailSender;
            _hub = hub;
        }

        public async Task<AssignByTaskResponse?> AssignByTaskAsync(string taskId, AssignByTaskModel model, string projectId, string assignerId)
        {
            try
            {
                var parseResult = Guid.TryParse(taskId, out var validId);
                if (parseResult == false)
                {
                    return null;
                }
                var task = await _db.Tasks.Include(t => t.TaskAssignments).FirstOrDefaultAsync(t => t.Id == validId);
                if (task == null) return null;

                var taskAssignmentIds = task.TaskAssignments.Select(ta => ta.AssignmentId);
                List<string> acceptedIds = [];
                List<TaskAssignment> taskAssignments = [];
                foreach (var assignmentId in model.AssignmentIds)
                {
                    if (taskAssignmentIds.Contains(assignmentId))
                    {
                        continue;
                    }
                    var taskAssignment = new TaskAssignment()
                    {
                        Id = TextUtils.CreateAssignmentId(task.Id.ToString(), assignmentId),
                        AssignedAt = DateTimeUtils.GetSeconds(),
                        AssignmentId = assignmentId,
                        AssignerId = assignerId,
                    };
                    taskAssignments.Add(taskAssignment);
                    acceptedIds.Add(assignmentId);
                }
                task.TaskAssignments.AddRange(taskAssignments);

                var assignedEmails = await _db.Assignments.AsNoTracking().Where(a => acceptedIds.Contains(a.Id)).Select(a => a.User.Email).ToListAsync();

                _ = System.Threading.Tasks.Task.Run(async () => await SendEmailToTaskAssignmentsByTask(task.Name, assignedEmails));

                var changeLog = new Models.ChangeLog()
                {
                    Log = LogDetail.Create(task.Id, $@"Assigned {acceptedIds.Count} member to task {HtmlHelper.EntityName(task.Name)}", ELogAction.Assign, EEntityType.Task).ToString(),
                    CreatedAt = DateTimeUtils.GetSeconds(),
                    ProjectId = projectId,
                    AssignmentId = assignerId
                };
                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                _ = System.Threading.Tasks.Task.Run(async () =>
                    {
                        await _hub.Clients.Group(projectId).SendAsync("ReceiveChangeLog", ChangeLogResponse.Create(changeLog));
                    });

                return AssignByTaskResponse.Create(task.Id.ToString(), acceptedIds, assignerId);
            }
            catch
            {
                return null;
            }
        }
        private async System.Threading.Tasks.Task SendEmailToTaskAssignmentsByTask(string taskName, IEnumerable<string> assignedEmails)
        {
            try
            {
                if (assignedEmails.Count() > 0)
                {
                    var subject = $"Task {taskName} was assigned to you";
                    var body = $"You have been assigned to task <b style='color: blue;'>{taskName}</b>. Please check your board for more information.";
                    foreach (var email in assignedEmails)
                    {
                        await _emailSender.SendEmailAsync(email, subject, body);
                    }
                }
            }
            catch
            {
                return;
            }
        }
        public async Task<AssignmentProfileResponse?> GetAssignmentProfileAsync(string assignmentId)
        {
            try
            {
                var assignment = await _db.Assignments.FindAsync(assignmentId);
                if (assignment == null)
                {
                    return null;
                }
                var res = AssignmentProfileResponse.Create(assignment);
                var joinedTasks = await _db.TaskAssignments
                    .Where(ta => ta.AssignmentId == assignmentId)
                    .Select(ta => JoinedTaskResponse.Create(ta.Task, ta.Task.TaskAssignments.Count, ta.Task.List.Name, ta.AssignedAt)).ToListAsync() ?? [];
                res.JoinedTasks = joinedTasks;
                return res;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<AssignmentResponse>?> GetAssignmentsByProject(string projectId)
        {
            try
            {
                var assignments = await _db.Assignments.Where(a => a.ProjectId == projectId).Include(a => a.User).Select(a => (AssignmentResponse)a).ToListAsync();
                if (assignments == null)
                {
                    return [];
                }
                return assignments;
            }
            catch
            {
                return null;
            }
        }

        private async System.Threading.Tasks.Task SendEmailToInvitedAssignments(IEnumerable<ProjectInvitation> invitations)
        {
            try
            {
                string subject = "Invitation to new project";
                string body = "You have been invited to a new project. Please sign up to get started.";
                foreach (var invitation in invitations)
                {
                    var email = invitation.InvitedEmail;
                    Console.WriteLine("\t\tSending to email: " + email);
                    await _emailSender.SendEmailAsync(email, subject, body);
                }
            }
            catch
            {
                return;
            }
        }
        public async Task<List<ExistedUserInvitationResult>?> InviteExistedMembersAsync(IEnumerable<ExistedMemberInviteModel> model, string invitedProjectId, string assignmentId)
        {
            try
            {
                // chuyển đổi kết quả chọn từ client thành dạng dictionary
                var dict = new Dictionary<string, ProjectSelectOptions>(model.Select(m => m.GetKeyValuePair()));

                // tìm kiếm project sẽ được mời vào
                var invitedProject = await _db.Projects.Include(p => p.ProjectInvitations).FirstOrDefaultAsync(p => p.Id == invitedProjectId);
                if (invitedProject == null) return null;

                // danh sách kết quả mời gồm userid và invitation
                List<ExistedUserInvitationResult> invitations = [];

                foreach (var option in dict)
                {
                    var projectId = option.Key;
                    var options = option.Value;

                    // lấy danh sách asignment id được chọn
                    var invitedAssignmentIds = options.SelectedAssignments.Select(a => a.Id);

                    /*
                    Tìm danh sách thành viên được mời theo mã dự án
                    - Nếu `option.IsAll == true` thì lấy hết thành viên trong dự án đó
                    - Nếu không, chọn những thành viên có `id` có tồn tại trong `invitedAssignmentIds`
                    */
                    List<Assignment> assignments =
                        await _db.Assignments
                            .AsNoTracking()
                            .Where(a => a.ProjectId == projectId && ((options.IsAll ?? false) || invitedAssignmentIds.Contains(a.Id)))
                            .Include(a => a.User)
                            .ToListAsync();

                    // tuỳ vào options mà xét lại permission
                    var joinOptions = options.SelectedAssignments
                        .Join(assignments,
                            sa => sa.Id,
                            a => a.Id,
                            (sa, a) => new
                            {
                                a.User,
                                Permission = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sa.Permission.Split('-')[1])
                            }
                        );
                    // thêm vào danh sách kết quả mời
                    invitations.AddRange(joinOptions.Select(o => new ExistedUserInvitationResult(o.User.Id, new ProjectInvitation()
                    {
                        Id = Guid.NewGuid().ToString(),
                        InvitedEmail = o.User.Email,
                        InvitedAt = DateTimeUtils.GetSeconds(),
                        InviterId = assignmentId,
                        Permission = (EPermission)Enum.Parse(typeof(EPermission), o.Permission),
                        ProjectId = projectId
                    })));
                }
                invitations = invitations.Distinct(new InvitationComparer()).ToList(); // lọc ra các lời mời đến cùng 1 người

                // lấy ra danh sách các thư mời vừa tạo
                var invites = invitations.Select(i => i.Invitation);

                // loại ra các thư mời đã được tạo sẵn trong dự án, sẽ có trường hợp đã mời rồi mà mời lại thì không cần thêm vào nữa
                invites = invites.Where(i => !invitedProject.ProjectInvitations.Any(ip => ip.InvitedEmail == i.InvitedEmail)).ToList();

                // gửi email cho các thành viên đã mời
                _ = System.Threading.Tasks.Task.Run(async () =>
                {
                    try { await SendEmailToInvitedAssignments(invites); }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine(ex.Message);
                        // send email to administrator
                    }
                });

                // thêm danh sách thư mời vào dự án và lưu vào cơ sở dữ liệu
                invitedProject.ProjectInvitations.AddRange(invites);
                await _db.SaveChangesAsync();

                return invitations;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<AssignmentResponse>?> GetAssignmentsFromAnotherProject(string currentProjectId, string otherProjectId, string userId)
        {
            try
            {
                // lấy thông tin dự án và các thành viên trong dự án, lát sau sẽ lọc ra các thành viên đã có rồi trong dự án
                var currentProject = await _db.Projects
                    .Include(p => p.Assignments)
                    .FirstOrDefaultAsync(p => p.Id == currentProjectId);
                if (currentProject == null)
                {
                    return null;
                }

                // lấy thông tin dự án và các thành viên của dự án cần lấy
                var otherProject = await _db.Projects
                    .Include(p => p.Assignments)
                    .ThenInclude(a => a.User)
                    .FirstOrDefaultAsync(p => p.Id == otherProjectId);
                if (otherProject == null)
                {
                    return null;
                }

                // loại thông tin thành viên là người yêu cầu ra
                var otherAssignments = otherProject.Assignments.Where(a => a.UserId != userId);

                // lấy ra danh sách các thành viên hiện có trong dự án gốc
                var currentAssignmentIds = currentProject.Assignments.Select(a => a.Id) ?? [];

                // loại các thành viên đã có trong dự án gốc, chỉ lấy các thành viên không có trong dự án gốc
                var result = otherAssignments
                    .Where(oa => !currentAssignmentIds.Any(caId => caId == TextUtils.CreateAssignmentId(currentProjectId, oa.UserId)))
                    .Select(oa => (AssignmentResponse)oa);

                return result.ToList();
            }
            catch
            {
                return null;
            }
        }

        public async Task<DeletedTaskAssignmentResponse?> DeleteTaskAssignment(string taskId, DeleteTaskAssignmentModel model)
        {
            try
            {
                var parseResult = Guid.TryParse(taskId, out var validId);
                if (!parseResult)
                {
                    return null;
                }

                // nếu có đang thực hiện ít nhất một subtask thì không xoá
                var isDoingSubtask = await _db.SubTasks.AsNoTracking().AnyAsync(s => s.TaskId == validId && s.AssignmentId == model.AssignmentId);
                if (isDoingSubtask)
                {
                    return null;
                }
                var taskAssignment = await _db.TaskAssignments.Include(t => t.Task).Include(t => t.Assignment).ThenInclude(a => a.User).FirstOrDefaultAsync(t => t.TaskId == validId && t.AssignmentId == model.AssignmentId);
                if (taskAssignment == null)
                {
                    return null;
                }
                _db.TaskAssignments.Remove(taskAssignment);

                _ = System.Threading.Tasks.Task.Run(async () =>
                {
                    // var email = await _db.Assignments.Select(a => new { a.Id, a.User.Email }).FirstOrDefaultAsync(a => a.Id == model.AssignmentId);
                    // var task = await _db.Tasks.FindAsync(validId);
                    // if (email == null || task == null) return;
                    await _emailSender.SendEmailAsync(taskAssignment.Assignment.User.Email, "Unassigned from task", $"You was unasssigned from task {taskAssignment.Task.Name}");
                });

                await _db.SaveChangesAsync();
                return new DeletedTaskAssignmentResponse() { TaskId = taskId, AssignmentId = model.AssignmentId };
            }
            catch
            {
                return null;
            }
        }

        public async Task<DeletedAssignmentResponse?> DeleteAssignmentAsync(string assignmentId, string deletedId)
        {
            try
            {
                var assignmentToDelete = await _db.Assignments
                    .Include(a => a.User)
                    .Include(a => a.TaskAssignments)
                    .FirstOrDefaultAsync(a => a.Id == deletedId);

                if (assignmentToDelete == null)
                {
                    return null;
                }

                var deleterAssignment = await _db.Assignments
                    .AsNoTracking()
                    .Select(a => new { a.Id, a.Permission, a.Project, a.User })
                    .FirstOrDefaultAsync(a => a.Id == assignmentId);

                if (deleterAssignment == null)
                {
                    return null;
                }

                // nếu người bị xoá là owner thì không thể xoá khỏi project
                if (assignmentToDelete.Permission == EPermission.Owner)
                {
                    return null;
                }

                // nếu không phải owner thì không được xoá admin
                if (assignmentToDelete.Permission == EPermission.Admin && deleterAssignment.Permission != EPermission.Owner)
                {
                    return null;
                }

                // _db.TaskAssignments.RemoveRange(assignmentToDelete.TaskAssignments);
                var relatedTaskAssignment = _db.TaskAssignments.Where(ta => ta.AssignmentId == assignmentToDelete.Id);

                _db.TaskAssignments.RemoveRange(relatedTaskAssignment);

                _db.Assignments.Remove(assignmentToDelete);

                var changeLog = new ChangeLog()
                {
                    ProjectId = assignmentToDelete.ProjectId,
                    AssignmentId = assignmentId,
                    Log = LogDetail.Create(assignmentId, $@"{HtmlHelper.EntityName(deleterAssignment.User.Email)} deleted assignment {HtmlHelper.EntityName(assignmentToDelete.User.Email, "deleted-assignment-email")}", ELogAction.Delete, EEntityType.Assignment).ToString(),
                    CreatedAt = DateTimeUtils.GetSeconds(),
                };

                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                _ = System.Threading.Tasks.Task.Run(async () => await _emailSender.SendEmailAsync(assignmentToDelete.User.Email, "Out of project", $"You was removed from the project {deleterAssignment.Project.Name}"));

                _ = System.Threading.Tasks.Task.Run(async () =>
                    {
                        await _hub.Clients.Group(changeLog.ProjectId).SendAsync("ReceiveChangeLog", ChangeLogResponse.Create(changeLog));
                    });

                return new DeletedAssignmentResponse() { AssignmentId = assignmentToDelete.Id };
            }
            catch
            {
                return null;
            }
        }

        public async Task<ChangePermissionResponse?> ChangePermissionAsync(string changerId, string projectId, string assignmentId, ChangePermissionModel model)
        {
            try
            {
                var assignment = await _db.Assignments.Include(a => a.User).Include(a => a.Project).FirstOrDefaultAsync(a => a.Id == assignmentId);
                if (assignment == null) return null;
                if (assignment.Permission == EPermission.Owner)
                {
                    return null; // Owner can't change permission to anybody else.
                }
                var oldPermission = assignment.Permission.ToString();
                assignment.Permission = (EPermission)Enum.Parse(typeof(EPermission), model.Permission);
                var changeLog = new ChangeLog()
                {
                    ProjectId = projectId,
                    AssignmentId = changerId,
                    Log = LogDetail.Create(assignmentId, $@"Change permission from ${oldPermission} to {model.Permission}", ELogAction.Update, EEntityType.Assignment).ToString(),
                    CreatedAt = DateTimeUtils.GetSeconds(),
                };

                _ = System.Threading.Tasks.Task.Run(async () => await _emailSender.SendEmailAsync(assignment.User.Email, "Change permission", $"You was changed from {oldPermission} to {model.Permission} in project {assignment.Project.Name}"));

                _ = System.Threading.Tasks.Task.Run(async () =>
                    {
                        await _hub.Clients.Group(changeLog.ProjectId).SendAsync("ReceiveChangeLog", ChangeLogResponse.Create(changeLog));
                    });
                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                return new ChangePermissionResponse() { AssignmentId = assignmentId, NewPermission = assignment.Permission.ToString() };
            }
            catch
            {
                return null;
            }
        }
    }
}