using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseForBoardDisplay;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Utils;

namespace backend_apis.Repositories
{
    public interface ITaskRepository
    {
        Task<(Models.Task?, string)> CreateTaskAsync(CreateTaskModel model, ProjectCookie cookie);
        Task<bool> ChangeTaskOrderAsync(ChangeTaskOrderModel model, string taskId, ProjectCookie cookie);
        Task<TaskDetailForBoardDisplay?> GetTaskDetailAsync(string taskId);
        Task<Models.Task?> UpdateTaskAsync(string taskId, UpdateTaskModel model, ProjectCookie cookie);
        Task<List<TaskResponseForBoardDisplay>?> DuplicateTaskAsync(DuplicateTaskModel model, string taskId, ProjectCookie cookie);
        Task<TaskResponseForBoardDisplay?> RestoreAsync(string taskId, ProjectCookie cookie);
        Task<Models.Task?> DeleteTaskAsync(string taskId, ProjectCookie cookie);
        Task<Models.Task?> MarkTaskAsync(string taskId, ProjectCookie cookie, MarkTaskModel model);
        Task<Models.TaskAssignment?> JoinTaskAsync(string taskId, ProjectCookie cookie);
        Task<Models.Task?> MoveTaskToTrashAsync(string taskId, ProjectCookie cookie);
        Task<UpdatedTaskResponse?> ResetTaskAsync(string taskId, ProjectCookie cookie, ResetTaskModel model);
        Task<List<RelatedTaskResponse>?> AddDependenciesAsync(string taskId, ProjectCookie cookie, AddTaskDependenciesModel model);
        Task<List<RelatedTaskResponse>?> GetDependenciesAsync(string taskId);
        Task<ReferenceTasks?> GetReferenceTasksAsync(string taskId);
        Task<List<RelatedTaskResponse>?> AddChildrenAsync(string taskId, ProjectCookie cookie, AddChildrenTaskModel model);
        Task<DeletedRelationshipResponse?> DeleteTaskRelationshipAsync(string taskId, ProjectCookie cookie, string relationship, string rid);

        // v2: remove reference tasks
        Task<Models.Task?> MoveTaskToTrashAsync_v2(string taskId, ProjectCookie cookie);

        // v2: remove reference tasks
        Task<Models.Task?> DeleteTaskAsync_v2(string taskId, ProjectCookie cookie);
    }
}