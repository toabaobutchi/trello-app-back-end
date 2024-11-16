using backend_apis.ApiModels;
using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseForBoardDisplay;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Data;
using backend_apis.Exceptions;
using backend_apis.Hubs;
using backend_apis.Models;
using backend_apis.Services;
using backend_apis.Utils;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ProjectManagerDbContext _db;
        private readonly IHubContext<ProjectHub> _hub;
        private readonly ChangeLogService _changeLogService;

        public TaskRepository(ProjectManagerDbContext db, IHubContext<ProjectHub> hub, ChangeLogService changeLogService)
        {
            _db = db;
            _hub = hub;
            _changeLogService = changeLogService;
        }

        private string SetTaskOrder(string startOrder, string newId)
        {
            if (string.IsNullOrEmpty(startOrder))
            {
                return newId;
            }
            return (startOrder + "," + newId).TrimEnd(',', ' ');
        }
        private Guid ValidateAndParseTaskId(string id)
        {
            var isValidTaskId = Guid.TryParse(id, out var validTaskId);
            if (!isValidTaskId)
            {
                throw new Exception();
            }
            return validTaskId;
        }
        private void ThrowIfNull<T>(T value)
        {
            if (value == null) throw new Exception();
        }
        private bool IsOverWIPLimit(List? list, int additionalCount = 0)
        {
            return list != null && list.WipLimit != null && list.WipLimit > 0 && list.TaskOrder.Split(',').Length + additionalCount >= list.WipLimit;
        }

        public async Task<bool> ChangeTaskOrderAsync(ChangeTaskOrderModel model, string taskId, ProjectCookie cookie)
        {
            try
            {
                var (projectId, assignmentId, _) = cookie.Destruct();
                var validTaskId = ValidateAndParseTaskId(taskId);
                var updatedTask = await _db.Tasks.FindAsync(validTaskId);
                ThrowIfNull(updatedTask);

                var newList = await _db.Lists.FirstOrDefaultAsync(l => l.Id == model.NewListId && l.ProjectId == projectId);
                ThrowIfNull(newList);

                string log = "";
                // trường hợp thay đổi trong cùng 1 cột / list
                if (model.NewListId == model.OldListId)
                {
                    // thay đổi 1 cột duy nhất và không cần cập nhật lại list id của task
                    newList.TaskOrder = model.NewTaskOrder;
                    log = $@"Change task order of {HtmlHelper.EntityName(updatedTask.Name, "task-name")} in the same list {HtmlHelper.EntityName(newList.Name, "list-name")}";
                }
                else
                {
                    // nếu có cài đặt wip thì phải kiểm tra
                    if (newList.WipLimit != null && newList.WipLimit != 0)
                    {
                        await _db.Entry(newList).Collection(l => l.Tasks).LoadAsync();
                        if (newList.Tasks.Where(t => t.IsDeleted == null || t.IsDeleted == false).Count() >= newList.WipLimit)
                        {
                            return false;
                        }
                    }
                    var oldList = await _db.Lists.FindAsync(model.OldListId);

                    ThrowIfNull(oldList);

                    oldList.TaskOrder = model.OldTaskOrder;
                    newList.TaskOrder = model.NewTaskOrder;
                    updatedTask.ListId = model.NewListId;
                    updatedTask.LastListUpdatedAt = DateTimeUtils.GetSeconds(); // cập nhật lại thời gian khi thay đổi cột

                    log = $@"Change task order of {HtmlHelper.EntityName(updatedTask.Name, "task-name")} in from list {HtmlHelper.EntityName(oldList.Name, "list-name")} to list {HtmlHelper.EntityName(newList.Name, "list-name")}";
                }

                // ghi change log
                var changeLog = Generator.CreateChangeLog(cookie, LogDetail.Create(updatedTask.Id, log, ELogAction.Update, EEntityType.Task));

                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                _ = _changeLogService.SendChangeLogAsync(projectId, changeLog);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(Models.Task?, string)> CreateTaskAsync(CreateTaskModel model, ProjectCookie cookie)
        {
            try
            {
                var (projectId, creatorId, _) = cookie.Destruct();
                var task = model.ToTask(creatorId);
                // tìm list sẽ chứa task
                var list = await _db.Lists.FindAsync(model.ListId);

                // không thể tạo task trong list đã đạt đủ wip limit
                // var isOverWIPLimit = list.WipLimit != null && list.WipLimit > 0 && list.TaskOrder.Split(',').Count() >= list.WipLimit;
                if (list == null || IsOverWIPLimit(list))
                {
                    return (null, "");
                }

                // kiểm tra xem có đang vi phạm WIP Limit hay không
                _db.Entry(list).Collection(d => d.Tasks).Load();
                if (list.Tasks.Where(t => t.IsDeleted == null || t.IsDeleted == false).Count() >= list.WipLimit)
                {
                    return (null, "");
                }
                else
                {
                    // cập nhật lại thứ tự task
                    var taskOrder = SetTaskOrder(list.TaskOrder, task.Id.ToString());
                    list.TaskOrder = taskOrder;

                    // ghi change log
                    var logDetail = LogDetail.Create(task.Id, $@"Create new task {HtmlHelper.EntityName(task.Name, "task-name")} in list {HtmlHelper.EntityName(list.Name, "list-name")}", ELogAction.Create, EEntityType.Task);
                    var changeLog = Generator.CreateChangeLog(cookie, logDetail);

                    await _db.ChangeLogs.AddAsync(changeLog);
                    await _db.Tasks.AddAsync(task);
                    await _db.SaveChangesAsync();

                    _ = _changeLogService.SendChangeLogAsync(projectId, changeLog);

                    return (task, list.TaskOrder);
                }
            }
            catch
            {
                return (null, "");
            }
        }


        private static Models.Task CreateDuplicatedTask(Models.Task originalTask, DuplicateTaskModel model, int index, string listId, string creatorId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                Name = $"{originalTask.Name} (Copy {index + 1})",
                CreatedAt = DateTimeUtils.GetSeconds(),
                CreatorId = creatorId,
                Priority = (model.InheritPriority != null && model.InheritPriority == true) ? originalTask.Priority : null,
                Description = (model.InheritDescription != null && model.InheritDescription == true) ? originalTask.Description : null,
                DueDate = (model.InheritDueDate != null && model.InheritDueDate == true) ? originalTask.DueDate : null,
                ListId = listId,
            };
        }
        public async Task<Models.Task?> DeleteTaskAsync(string taskId, ProjectCookie cookie)
        {
            try
            {
                var (projectId, assignmentId, _) = cookie.Destruct();
                var validId = ValidateAndParseTaskId(taskId);

                var task = await _db.Tasks
                    .Include(t => t.Comments)
                    .Include(t => t.TaskAssignments)
                    .FirstOrDefaultAsync(t => t.Id == validId && t.List != null && t.List.ProjectId == projectId);

                ThrowIfNull(task);

                // ghi change log
                var changeLog = Generator.CreateChangeLog(cookie, LogDetail.Create(task.Id, $@"Delete permantly task {HtmlHelper.EntityName(task.Name, "task-name")}", ELogAction.Delete, EEntityType.Task));

                _db.Comments.RemoveRange(task.Comments);
                _db.TaskAssignments.RemoveRange(task.TaskAssignments);
                var result = _db.Tasks.Remove(task);
                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                _ = _changeLogService.SendChangeLogAsync(projectId, changeLog);
                return result.Entity;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<TaskResponseForBoardDisplay>?> DuplicateTaskAsync(DuplicateTaskModel model, string taskId, ProjectCookie cookie)
        {
            try
            {
                var (projectId, assignmentId, _) = cookie.Destruct();
                var validId = ValidateAndParseTaskId(taskId);
                var task = await _db.Tasks
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == validId && t.List != null && t.List.ProjectId == projectId);

                ThrowIfNull(task);

                var placedList = await _db.Lists.FirstOrDefaultAsync(l => l.Id == model.ListId);
                if (placedList == null || IsOverWIPLimit(placedList, model.DuplicateTaskCount))
                {
                    return null;
                }
                List<Models.Task> clonedTasks = [];
                string taskOrderToAppend = "";
                for (int i = 0; i < model.DuplicateTaskCount; i++)
                {
                    var clonedTask = CreateDuplicatedTask(task, model, i, placedList.Id, assignmentId);
                    clonedTasks.Add(clonedTask);
                    taskOrderToAppend = SetTaskOrder(taskOrderToAppend, clonedTask.Id.ToString());
                }

                await _db.Tasks.AddRangeAsync(clonedTasks);
                placedList.TaskOrder = SetTaskOrder(placedList.TaskOrder, taskOrderToAppend);

                // ghi change log

                var logDetail = LogDetail.Create(task.Id, $@"Created {HtmlHelper.EntityNumber(model.DuplicateTaskCount, "duplicate-task-count")} copies of task {HtmlHelper.EntityName(task.Name, "task-name")}", ELogAction.Create, EEntityType.Task);

                var changeLog = Generator.CreateChangeLog(cookie, logDetail);

                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                _ = _changeLogService.SendChangeLogAsync(projectId, changeLog);

                var result = placedList.Tasks.Select(TaskResponseForBoardDisplay.Create).ToList();
                return result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<TaskDetailForBoardDisplay?> GetTaskDetailAsync(string taskId)
        {
            try
            {
                var validId = ValidateAndParseTaskId(taskId);
                var task = await (from t in _db.Tasks
                                  where t.Id == validId
                                  select new
                                  {
                                      Task = t,
                                      listName = t.List.Name,
                                      attachmentCount = t.Attachments.Count,
                                      assignmentIds = t.TaskAssignments.Select(ta => ta.AssignmentId),
                                      subTasks = t.SubTasks
                                  }).AsNoTracking().FirstOrDefaultAsync();
                if (task == null)
                {
                    return null;
                }
                var result = TaskDetailForBoardDisplay.Create(task.Task, task.listName, task.attachmentCount, task.assignmentIds, task.subTasks);
                return result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<TaskAssignment?> JoinTaskAsync(string taskId, ProjectCookie cookie)
        {
            try
            {
                var (projectId, assignmentId, _) = cookie.Destruct();
                var validId = ValidateAndParseTaskId(taskId);
                var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == validId && t.List.ProjectId == projectId);
                if (task == null)
                {
                    return null;
                }
                var taskAssignment = new TaskAssignment()
                {
                    Id = TextUtils.CreateAssignmentId(task.Id.ToString(), assignmentId),
                    AssignmentId = assignmentId,
                    AssignedAt = DateTimeUtils.GetSeconds(),
                    AssignerId = null,
                };

                var changeLog = Generator.CreateChangeLog(cookie, LogDetail.Create(task.Id, $@"Join task {HtmlHelper.EntityName(task.Name, "task-name")}", ELogAction.Join, EEntityType.Task));

                task.TaskAssignments.Add(taskAssignment);
                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                _ = _changeLogService.SendChangeLogAsync(projectId, changeLog);
                return taskAssignment;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Models.Task?> MarkTaskAsync(string taskId, ProjectCookie cookie, MarkTaskModel model)
        {
            try
            {
                var (projectId, assignmentId, _) = cookie.Destruct();
                var validId = ValidateAndParseTaskId(taskId);
                var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == validId && t.List.ProjectId == projectId);
                ThrowIfNull(task);

                model.Update(ref task);
                string log = "";
                if (model.IsCompleted != null)
                {
                    log = $@"Task {HtmlHelper.EntityName(task.Name, "task-name")} was marked as " + (model.IsCompleted.Value ? "completed" : "uncompleted");
                }
                else if (model.IsMarkedNeedHelp != null)
                {
                    log = $@"Task {HtmlHelper.EntityName(task.Name, "task-name")} was marked as " + (model.IsMarkedNeedHelp.Value ? "need help" : "not need help");
                }
                else if (model.IsReOpened != null)
                {
                    log = $@"Task {HtmlHelper.EntityName(task.Name, "task-name")} was re-opened";
                }

                var changeLog = Generator.CreateChangeLog(cookie, LogDetail.Create(task.Id, log, ELogAction.Update, EEntityType.Task));

                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                _ = _changeLogService.SendChangeLogAsync(projectId, changeLog);

                return task;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Models.Task?> MoveTaskToTrashAsync(string taskId, ProjectCookie cookie)
        {
            try
            {
                var (projectId, assignmentId, _) = cookie.Destruct();
                var validId = ValidateAndParseTaskId(taskId);
                var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == validId && t.List.ProjectId == projectId);
                if (task == null) return null;
                task.IsDeleted = true;
                task.DeleterId = assignmentId;
                task.DeletedAt = DateTimeUtils.GetSeconds();

                var changeLog = Generator.CreateChangeLog(cookie, LogDetail.Create(task.Id, $@"Move task {HtmlHelper.EntityName(task.Name)} to trash", ELogAction.Delete, EEntityType.Task));

                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                _ = _changeLogService.SendChangeLogAsync(projectId, changeLog);
                return task;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Models.Task?> UpdateTaskAsync(string taskId, UpdateTaskModel model, ProjectCookie cookie)
        {
            try
            {
                var (projectId, assignmentId, _) = cookie.Destruct();
                var validId = ValidateAndParseTaskId(taskId);
                Models.Task? task = null;
                if (model.DueDate != null)
                {
                    // kiem tra project due date
                    task = await _db.Tasks
                        .Include(t => t.List)
                            .ThenInclude(l => l.Project)
                        .FirstOrDefaultAsync(t => t.Id == validId);

                    ThrowIfNull(task);

                    if (task.List?.Project.DueDate != null && task.List.Project.DueDate < model.DueDate)
                    {
                        throw new OverDueDateException("Task due date is over than project due date");
                    }
                }
                else
                {
                    task = await _db.Tasks.FindAsync(validId);
                }
                if (task == null)
                {
                    return null;
                }
                model.Update(ref task);

                var changeLog = Generator.CreateChangeLog(cookie, LogDetail.Create(task.Id, $@"Update task {HtmlHelper.EntityName(task.Name)}", ELogAction.Update, EEntityType.Task));

                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                _ = _changeLogService.SendChangeLogAsync(projectId, changeLog);
                return task;
            }
            catch
            {
                return null;
            }
        }

        public async Task<UpdatedTaskResponse?> ResetTaskAsync(string taskId, ProjectCookie cookie, ResetTaskModel model)
        {
            try
            {
                var (projectId, assignmentId, _) = cookie.Destruct();
                var validId = ValidateAndParseTaskId(taskId);
                var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == validId && t.List != null && t.List.ProjectId == projectId);
                ThrowIfNull(task);
                model.Reset(ref task);

                var changeLog = Generator.CreateChangeLog(cookie, LogDetail.Create(task.Id, $@"Reset task {HtmlHelper.EntityName(task.Name)}", ELogAction.Update, EEntityType.Task));

                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                _ = _changeLogService.SendChangeLogAsync(projectId, changeLog);

                return UpdatedTaskResponse.Create(task);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<RelatedTaskResponse>?> AddDependenciesAsync(string taskId, ProjectCookie cookie, AddTaskDependenciesModel model)
        {
            try
            {
                var (projectId, assignmentId, _) = cookie.Destruct();
                var validId = ValidateAndParseTaskId(taskId);

                var task = await _db.Tasks
                    .AsSplitQuery()
                    .Include(t => t.ParentDependentTasks)
                    .Include(t => t.ChildDependentTasks)
                    .FirstOrDefaultAsync(t => t.Id == validId && t.List != null && t.List.ProjectId == projectId);

                ThrowIfNull(task);

                // lấy danh sách id dependency hiện tại của task cần thêm
                var currentDependenciesIds = task.ParentDependentTasks.Select(t => t.DependentTaskId.ToString());
                var currentChildrenIds = task.ChildDependentTasks.Select(t => t.TaskId.ToString());

                // loại ra các id của dependency hoặc children mà task cần thêm đã có
                // nếu vừa làm children vừa làm dependency thì không được
                var dependenciesToAdd = model.Dependencies.Where(id => !currentDependenciesIds.Contains(id) && !currentChildrenIds.Contains(id));

                // thêm các id vào danh sách task cha
                List<RelatedTaskResponse> results = [];
                foreach (var dependencyId in dependenciesToAdd)
                {
                    var dependencyParseResult = Guid.TryParse(dependencyId, out var validDependencyId);
                    if (!dependencyParseResult)
                    {
                        continue;
                    }
                    var dependencyTask = await _db.Tasks.AsNoTracking().FirstOrDefaultAsync(t =>
                        t.Id == validDependencyId &&
                        t.List != null &&
                        t.List.ProjectId == projectId &&
                        (t.IsDeleted == null || t.IsDeleted.Value == false));
                    if (dependencyTask == null || dependencyTask.Id == task.Id)
                    {
                        continue;
                    }
                    var dependencyDetail = new Models.TaskDependenceDetail()
                    {
                        TaskId = task.Id,
                        DependentTaskId = dependencyTask.Id
                    };
                    task.ParentDependentTasks.Add(dependencyDetail);
                    results.Add(RelatedTaskResponse.Create(dependencyTask));
                }

                var changeLog = Generator.CreateChangeLog(cookie, LogDetail.Create(task.Id, $@"Add dependencies to task {HtmlHelper.EntityName(task.Name)}", ELogAction.Update, EEntityType.Task));

                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                _ = _changeLogService.SendChangeLogAsync(projectId, changeLog);

                return results;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<RelatedTaskResponse>?> GetDependenciesAsync(string taskId)
        {
            try
            {
                var validId = ValidateAndParseTaskId(taskId);
                var relatedTasks = await _db.TaskDependenceDetails.Where(t => t.TaskId == validId).Select(t => RelatedTaskResponse.Create(t.DependentTask)).ToListAsync();
                return relatedTasks;
            }
            catch
            {
                return null;
            }
        }
        public async Task<ReferenceTasks?> GetReferenceTasksAsync(string taskId)
        {
            try
            {
                var validId = ValidateAndParseTaskId(taskId);
                var depTasks = await _db.TaskDependenceDetails
                    .Where(t => t.TaskId == validId)
                    .Select(t => RelatedTaskResponse.Create(t.DependentTask))
                    .ToListAsync();
                if (depTasks == null)
                {
                    return null;
                }
                var childTasks = await _db.TaskDependenceDetails
                    .Where(t => t.DependentTaskId == validId)
                    .Select(t => RelatedTaskResponse.Create(t.Task))
                    .ToListAsync();
                if (childTasks == null)
                {
                    return null;
                }
                var referenceTasks = new ReferenceTasks()
                {
                    Dependencies = depTasks,
                    ChildTasks = childTasks
                };
                return referenceTasks;
            }
            catch
            {
                return null;
            }
        }
        public async Task<List<RelatedTaskResponse>?> AddChildrenAsync(string taskId, ProjectCookie cookie, AddChildrenTaskModel model)
        {
            try
            {
                var (projectId, assignmentId, _) = cookie.Destruct();
                var validId = ValidateAndParseTaskId(taskId);

                var task = await _db.Tasks
                    .AsSplitQuery()
                    .Include(t => t.ParentDependentTasks)
                    .Include(t => t.ChildDependentTasks)
                    .FirstOrDefaultAsync(t => t.Id == validId && t.List != null && t.List.ProjectId == projectId);

                ThrowIfNull(task);

                // lấy danh sách id dependency hiện tại của task cần thêm
                var currentDependenciesIds = task.ParentDependentTasks.Select(t => t.DependentTaskId.ToString());
                var currentChildrenIds = task.ChildDependentTasks.Select(t => t.TaskId.ToString());

                // loại ra các id của dependency hoặc children mà task cần thêm đã có
                // task cần thêm thì phải chưa có quan hệ với task hiện tại (1 trong 2 quan hệ sẽ không đạt và bị loại ra)
                var childrenToAdd = model.Children.Where(id => !currentDependenciesIds.Contains(id) && !currentChildrenIds.Contains(id));

                // thêm các id vào danh sách task cha
                List<RelatedTaskResponse> results = [];
                foreach (var childId in childrenToAdd)
                {
                    var childParseResult = Guid.TryParse(childId, out var validChildId);
                    if (!childParseResult)
                    {
                        continue;
                    }
                    var childTask = await _db.Tasks.AsNoTracking().FirstOrDefaultAsync(t =>
                        t.Id == validChildId &&
                        t.List != null &&
                        t.List.ProjectId == projectId &&
                        (t.IsDeleted == null || t.IsDeleted.Value == false));

                    if (childTask == null || childTask.Id == task.Id)
                    {
                        continue;
                    }
                    var dependencyDetail = new Models.TaskDependenceDetail()
                    {
                        TaskId = childTask.Id,
                        DependentTaskId = task.Id
                    };
                    task.ChildDependentTasks.Add(dependencyDetail);
                    results.Add(RelatedTaskResponse.Create(childTask));
                }

                var changeLog = Generator.CreateChangeLog(cookie, LogDetail.Create(task.Id, $@"Add children to task {HtmlHelper.EntityName(task.Name)}", ELogAction.Update, EEntityType.Task));

                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                _ = _changeLogService.SendChangeLogAsync(projectId, changeLog);

                return results;
            }
            catch
            {
                return null;
            }
        }

        public async Task<DeletedRelationshipResponse?> DeleteTaskRelationshipAsync(string taskId, ProjectCookie cookie, string relationship, string rid)
        {
            try
            {
                var (projectId, assignmentId, _) = cookie.Destruct();
                var validTaskId = ValidateAndParseTaskId(taskId);
                var validRid = ValidateAndParseTaskId(rid);

                TaskDependenceDetail? deletedDetail = await _db.TaskDependenceDetails
                         .Include(t => t.Task)
                         .Include(t => t.DependentTask)
                         .FirstOrDefaultAsync(t => (t.TaskId == validTaskId && t.DependentTaskId == validRid) || (t.TaskId == validRid && t.DependentTaskId == validTaskId));

                ThrowIfNull(deletedDetail);

                var changeLog = Generator.CreateChangeLog(cookie, LogDetail.Create(taskId, $@"Remove {relationship} between task {HtmlHelper.EntityName(deletedDetail.Task.Name)} and {HtmlHelper.EntityName(deletedDetail.DependentTask.Name)}", ELogAction.Delete, EEntityType.Task));

                await _db.ChangeLogs.AddAsync(changeLog);
                var deleted = _db.TaskDependenceDetails.Remove(deletedDetail);
                await _db.SaveChangesAsync();

                _ = _changeLogService.SendChangeLogAsync(projectId, changeLog);

                return DeletedRelationshipResponse.Create(deleted.Entity, relationship == "dependencies" ? DeletedRelationshipResponseType.Dependencies : DeletedRelationshipResponseType.Children);
            }
            catch
            {
                return null;
            }
        }

        public async Task<Models.Task?> MoveTaskToTrashAsync_v2(string taskId, ProjectCookie cookie)
        {
            try
            {
                var (projectId, assignmentId, _) = cookie.Destruct();
                var validId = ValidateAndParseTaskId(taskId);
                var task = await _db.Tasks
                    .AsSplitQuery()
                    .Include(t => t.ChildDependentTasks)
                    .Include(t => t.ParentDependentTasks)
                    .FirstOrDefaultAsync(t => t.Id == validId && t.List != null && t.List.ProjectId == projectId);

                ThrowIfNull(task);

                task.IsDeleted = true;
                task.DeleterId = assignmentId;
                task.DeletedAt = DateTimeUtils.GetSeconds();

                task.ChildDependentTasks.Clear();
                task.ParentDependentTasks.Clear();

                var changeLog = Generator.CreateChangeLog(cookie, LogDetail.Create(task.Id, $@"Move task {HtmlHelper.EntityName(task.Name)} to trash", ELogAction.Delete, EEntityType.Task));

                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                _ = _changeLogService.SendChangeLogAsync(projectId, changeLog);
                return task;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Models.Task?> DeleteTaskAsync_v2(string taskId, ProjectCookie cookie)
        {
            try
            {
                var (projectId, assignmentId, _) = cookie.Destruct();
                var validId = ValidateAndParseTaskId(taskId);
                var task = await _db.Tasks
                    .AsSplitQuery()
                    .Include(t => t.ParentDependentTasks)
                    .Include(t => t.ChildDependentTasks)
                    .Include(t => t.Comments)
                    .Include(t => t.TaskAssignments)
                    .FirstOrDefaultAsync(t => t.Id == validId && t.List != null && t.List.ProjectId == projectId);

                if (task == null) return null;

                var changeLog = Generator.CreateChangeLog(cookie, LogDetail.Create(task.Id, $@"Delete permantly task {HtmlHelper.EntityName(task.Name, "task-name")}", ELogAction.Delete, EEntityType.Task));

                _db.Comments.RemoveRange(task.Comments);
                task.ChildDependentTasks.Clear();
                task.ParentDependentTasks.Clear();
                _db.TaskAssignments.RemoveRange(task.TaskAssignments);
                var result = _db.Tasks.Remove(task);
                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();

                _ = _changeLogService.SendChangeLogAsync(projectId, changeLog);
                return result.Entity;
            }
            catch
            {
                return null;
            }
        }

        public async Task<TaskResponseForBoardDisplay?> RestoreAsync(string taskId, ProjectCookie cookie)
        {
            try
            {
                var projectId = cookie.ProjectId;
                var validId = ValidateAndParseTaskId(taskId);
                var task = await _db.Tasks
                    .FirstOrDefaultAsync(t => t.Id == validId && t.List != null && t.List.ProjectId == projectId);

                if (task == null) return null;
                task.IsDeleted = false;
                task.DeleterId = null;
                task.DeletedAt = null;
                await _db.SaveChangesAsync();
                return TaskResponseForBoardDisplay.Create(task);
            }
            catch
            {
                return null;
            }
        }
    }
}