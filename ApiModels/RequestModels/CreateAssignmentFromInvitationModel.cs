using backend_apis.Models;
using backend_apis.Utils;

namespace backend_apis.ApiModels.RequestModels
{
    public class CreateAssignmentFromInvitationModel
    {
        public string InvitationId { get; set; } = null!;

        public Assignment Create(ProjectInvitation invitation)
        {
            return new Assignment()
            {
                Id = TextUtils.CreateAssignmentId(invitation.ProjectId, invitation.InvitedEmail)
            };
        }
    }
}