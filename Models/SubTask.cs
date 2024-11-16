namespace backend_apis.Models
{
    public class SubTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public long CreatedAt { get; set; }
        public Guid TaskId { get; set; }
        public bool IsCompleted { get; set; }
        public long? CompletedAt { get; set; }
        public string? AssignmentId { get; set; }
        public long? AssignedAt { get; set; }
        public string? AssignerId { get; set; }
        public Assignment? Assigner { get; set; }
        public Task Task { get; set; } = null!;
        public Assignment? Assignment { get; set; }
    }
}