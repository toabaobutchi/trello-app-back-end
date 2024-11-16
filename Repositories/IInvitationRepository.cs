using backend_apis.ApiModels.ResponseModels;

namespace backend_apis.Repositories
{
    public interface IInvitationRepository
    {
        Task<InvitationResult?> HandleProjectInvitationAsync(string invitationId, string action);
    }
}