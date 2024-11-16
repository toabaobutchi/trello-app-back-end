using backend_apis.ApiModels;
using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;

namespace backend_apis.Repositories
{
    public interface IAssignmentRepository
    {
        Task<List<AssignmentResponse>?> GetAssignmentsByProject(string projectId);
        Task<List<ExistedUserInvitationResult>?> InviteExistedMembersAsync(IEnumerable<ExistedMemberInviteModel> model, string invitedProjectId, string assignmentId);
        Task<AssignmentProfileResponse?> GetAssignmentProfileAsync(string assignmentId);
        Task<AssignByTaskResponse?> AssignByTaskAsync(string taskId, AssignByTaskModel model, string projectId, string assignerId);
        Task<List<AssignmentResponse>?> GetAssignmentsFromAnotherProject(string currentProjectId, string otherProjectId, string userId);
        Task<DeletedTaskAssignmentResponse?> DeleteTaskAssignment(string taskId, DeleteTaskAssignmentModel model);
        Task<DeletedAssignmentResponse?> DeleteAssignmentAsync(string assignmentId, string deletedId);
        Task<ChangePermissionResponse?> ChangePermissionAsync(string changerId, string projectId, string assignmentId, ChangePermissionModel model);
    }
}