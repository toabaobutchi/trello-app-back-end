namespace backend_apis.Models
{
    public class User
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? DisplayName { get; set; }
        public long JoinAt { get; set; }
        public string? Avatar { get; set; }
        public long? LastNotificationView { get; set; }
        public List<Workspace> Workspaces { get; set; } = new();
        public List<Assignment> Assignments { get; set; } = new();
        public List<ProjectRequest> Requests { get; set; } = new();
    }
}