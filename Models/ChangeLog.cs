namespace backend_apis.Models
{
    public class ChangeLog
    {
        public int Id { get; set; }
        public string ProjectId { get; set; } = null!;
        public Project Project { get; set; } = null!;
        public string? AssignmentId { get; set; }
        public Assignment? Assignment { get; set; }
        public string? Log { get; set; }
        public long CreatedAt { get; set; }
    }
}