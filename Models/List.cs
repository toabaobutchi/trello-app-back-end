namespace backend_apis.Models
{
    public class List
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? TaskOrder { get; set; }
        public long CreatedAt { get; set; }
        public int? WipLimit { get; set; }
        public string ProjectId { get; set; } = null!;
        public Project Project { get; set; } = null!;
        public List<Task> Tasks { get; set; } = [];
    }
}