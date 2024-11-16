using System.Globalization;
using backend_apis.Models;
using backend_apis.Utils;

namespace backend_apis.ApiModels.RequestModels
{
    public class InvitationModel
    {
        public string Email { get; set; } = null!;
        public string Permission { get; set; } = null!;

        public ProjectInvitation ToProjectInvitation()
        {
            return new ProjectInvitation()
            {
                Id = Guid.NewGuid().ToString(),
                InvitedEmail = Email,
                InvitedAt = DateTimeUtils.GetSeconds(),
                Permission = (EPermission)Enum.Parse(typeof(EPermission), CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Permission))
            };
        }
    }
}