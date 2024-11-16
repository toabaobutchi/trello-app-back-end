using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Models;
using backend_apis.Utils;

namespace backend_apis.Repositories
{
    public interface ISubtaskRepository
    {
        Task<List<SubTask>?> CreateSubtasksAsync(CreateSubtaskModel model);
        Task<bool?> ChangeSubtaskStatusAsync(ChangeSubtaskStatusModel model, int subTaskId, ProjectCookie cookie);
        Task<SubTask?> DeleleSubtaskAsync(int subTaskId, string permission);
        Task<string?> ChangeSubtaskNameAsync(string name, int subtaskId, string permission);
        Task<JoinSubtaskResponse?> JoinSubtaskAsync(int subtaskId, string assignmentId);
        Task<AssignSubtaskResponse?> AssignSubtaskAsync(int subtaskId, string assignerId, AssignSubtaskModel model);
        Task<UnassignSubtaskResponse?> RemoveAssignmentFromSubtaskAsync(int subtaskId, UnassignSubtaskModel model);
    }
}