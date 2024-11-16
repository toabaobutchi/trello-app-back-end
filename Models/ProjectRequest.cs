using Microsoft.EntityFrameworkCore;

namespace backend_apis.Models
{
    [PrimaryKey(nameof(ProjectId), nameof(RequesterId))]
    public class ProjectRequest
    {
        public string ProjectId { get; set; } = null!;
        public string RequesterId { get; set; } = null!;
        public long RequestedAt { get; set; }
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Project Project { get; set; } = null!;
        public User Requester { get; set; } = null!;
    }
}