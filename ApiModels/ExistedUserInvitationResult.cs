using backend_apis.Models;

namespace backend_apis.ApiModels
{
    public sealed record ExistedUserInvitationResult(string UserId, ProjectInvitation Invitation)
    {
    }
}