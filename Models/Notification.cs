using System.ComponentModel.DataAnnotations.Schema;

namespace backend_apis.Models
{
    public class Notification
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public long CreatedAt { get; set; }
        [ForeignKey(nameof(Project))]
        public string ProjectId { get; set; } = null!;
        public Project Project { get; set; } = null!;
        public ENotificationScope Scope { get; set; }
        public EChangeAgent Agent { get; set; }
        public string? AgentId { get; set; }
    }
}