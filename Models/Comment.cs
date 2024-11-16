using Microsoft.EntityFrameworkCore;

namespace backend_apis.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public long CommentAt { get; set; }
        public Guid TaskId { get; set; }
        public string AssignmentId { get; set; } = null!;
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Task Task { get; set; } = null!;
        public Assignment Assignment { get; set; } = null!;
    }
}