using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Data;
using backend_apis.Models;
using backend_apis.Utils;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Repositories
{
    public class SubtaskRepository : ISubtaskRepository
    {
        private readonly ProjectManagerDbContext _db;

        public SubtaskRepository(ProjectManagerDbContext db)
        {
            _db = db;
        }

        public async Task<AssignSubtaskResponse?> AssignSubtaskAsync(int subtaskId, string assignerId, AssignSubtaskModel model)
        {
            try
            {
                // tải dữ liệu về subtask
                var subtask = await _db.SubTasks
                    .Include(s => s.Task)
                    .ThenInclude(t => t.TaskAssignments)
                    .FirstOrDefaultAsync(s => s.Id == subtaskId);

                if (subtask == null)
                {
                    return null;
                }
                var isNewAssignment = false;
                // kiểm tra xem người nhận subtask có phải là thành viên của task hay không
                if (!subtask.Task.TaskAssignments.Any(ta => ta.AssignmentId == model.AssignmentId))
                {
                    isNewAssignment = true;
                    subtask.Task.TaskAssignments.Add(new TaskAssignment()
                    {
                        Id = TextUtils.CreateAssignmentId(subtask.TaskId.ToString(), model.AssignmentId),
                        AssignmentId = model.AssignmentId,
                        AssignedAt = DateTimeUtils.GetSeconds(),
                        AssignerId = assignerId
                    });
                }
                subtask.AssignmentId = model.AssignmentId;
                subtask.AssignedAt = DateTimeUtils.GetSeconds();
                subtask.AssignerId = assignerId;
                await _db.SaveChangesAsync();
                return AssignSubtaskResponse.Create(subtask, isNewAssignment);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string?> ChangeSubtaskNameAsync(string name, int subtaskId, string permission)
        {
            try
            {
                var subtask = await _db.SubTasks.FindAsync(subtaskId);
                if (subtask == null)
                {
                    return null;
                }
                if (permission == ContextResponse.Admin || permission == ContextResponse.Owner)
                {
                    subtask.Title = name;
                    await _db.SaveChangesAsync();
                    return name;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool?> ChangeSubtaskStatusAsync(ChangeSubtaskStatusModel model, int subTaskId, ProjectCookie cookie)
        {
            try
            {
                var subtask = await _db.SubTasks.FindAsync(subTaskId);
                if (subtask == null)
                {
                    return null;
                }
                if (subtask.AssignmentId == cookie.AssignmentId || cookie.Permission == ContextResponse.Admin || cookie.Permission == ContextResponse.Owner)
                {
                    subtask.IsCompleted = model.IsCompleted;
                    subtask.CompletedAt = model.IsCompleted ? DateTimeUtils.GetSeconds() : null;
                    await _db.SaveChangesAsync();
                    return subtask.IsCompleted;
                }
                else return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<SubTask>?> CreateSubtasksAsync(CreateSubtaskModel model)
        {
            try
            {
                var taskId = model.TaskId;
                var parseResult = Guid.TryParse(taskId, out var validTaskId);
                var task = _db.Tasks.Find(validTaskId);
                if (task == null)
                {
                    return null;
                }
                task.SubTasks.AddRange(model.ToSubtasks());
                await _db.SaveChangesAsync();
                return task.SubTasks;
            }
            catch
            {
                return null;
            }
        }

        public async Task<SubTask?> DeleleSubtaskAsync(int subTaskId, string permission)
        {
            try
            {
                var subTask = await _db.SubTasks.FindAsync(subTaskId);
                if (subTask == null) return null;
                if (permission == ContextResponse.Admin || permission == ContextResponse.Owner)
                {
                    _db.SubTasks.Remove(subTask);
                    await _db.SaveChangesAsync();
                    return subTask;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<JoinSubtaskResponse?> JoinSubtaskAsync(int subtaskId, string assignmentId)
        {
            try
            {
                // tải dữ liệu về subtask
                var subtask = await _db.SubTasks
                    .Include(s => s.Task)
                    .ThenInclude(t => t.TaskAssignments)
                    .FirstOrDefaultAsync(s => s.Id == subtaskId);

                // trường hợp không tìm thấy subtask hoặc subtask đã có người thực hiện
                if (subtask == null || subtask.AssignmentId != null)
                {
                    return null;
                }

                // kiểm tra xem người nhận subtask có phải là thành viên của task hay không
                if (!subtask.Task.TaskAssignments.Any(ta => ta.AssignmentId == assignmentId))
                {
                    return null;
                }
                subtask.AssignmentId = assignmentId;
                subtask.AssignedAt = DateTimeUtils.GetSeconds();
                await _db.SaveChangesAsync();
                return JoinSubtaskResponse.Create(subtask);
            }
            catch
            {
                return null;
            }
        }

        public async Task<UnassignSubtaskResponse?> RemoveAssignmentFromSubtaskAsync(int subtaskId, UnassignSubtaskModel model)
        {
            try
            {
                var subtask = await _db.SubTasks.FindAsync(subtaskId);
                if (subtask == null || subtask.AssignmentId == null || subtask.AssignmentId != model.AssignmentId)
                {
                    return null;
                }
                subtask.AssignmentId = null;
                subtask.AssignedAt = null;
                await _db.SaveChangesAsync();
                return UnassignSubtaskResponse.Create(subtask);
            }
            catch
            {
                return null;
            }
        }
    }
}