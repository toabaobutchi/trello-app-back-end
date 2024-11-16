using System.ComponentModel.DataAnnotations.Schema;

namespace backend_apis.Models
{
    public class ProjectInvitation
    {
        public string Id { get; set; } = null!;
        public string InvitedEmail { get; set; } = null!;
        public EPermission Permission { get; set; }
        public long InvitedAt { get; set; }
        public string ProjectId { get; set; } = null!;
        [ForeignKey(nameof(Inviter))]
        public string? InviterId { get; set; }
        public Assignment? Inviter { get; set; }
        public Project Project { get; set; } = null!;
    }
}