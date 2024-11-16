namespace backend_apis.Models
{
    public class ProjectComment
    {
        public string Id { get; set; } = null!;
        public string Content { get; set; } = null!;
        public long CommentAt { get; set; }
        public string ProjectId { get; set; } = null!;
        public string? AssignmentId { get; set; } = null!;
        public Project Project { get; set; } = null!;
        public Assignment? Assignment { get; set; } = null!;
    }
}