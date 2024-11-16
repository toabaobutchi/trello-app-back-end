using backend_apis.ApiModels;
using backend_apis.Models;

namespace backend_apis.Utils
{
    public static class Generator
    {
        public static ChangeLog CreateChangeLog(ProjectCookie cookie, LogDetail detail)
        {
            return new ChangeLog()
            {
                AssignmentId = cookie.AssignmentId,
                CreatedAt = DateTimeUtils.GetSeconds(),
                ProjectId = cookie.ProjectId,
                Log = detail.ToString(),
            };
        }
        
        public static Email CreateProjectInvitationEmail(ProjectInvitationEmail invitationEmail)
        {
            return new()
            {
                ToEmail = invitationEmail.InvitedEmail,
                Subject = "Invitation to project",
                Body = "You were invited to a project. Sign up to get started."
            };
        }
    }
}