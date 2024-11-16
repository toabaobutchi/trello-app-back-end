namespace backend_apis.Models
{
    public class Attachment
    {
        public int Id { get; set; }
        public string? DisplayText { get; set; }
        public string Link { get; set; } = null!;
        public long CreatedAt { get; set; }
        public Guid TaskId { get; set; }
        public string? AssignmentId { get; set; }
        public Task Task { get; set; } = null!;
        public Assignment? Assignment { get; set; }
    }
}