namespace backend_apis.Models
{
    public class Project
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public long CreatedAt { get; set; }
        public long? DueDate { get; set; }
        public string? Color { get; set; }
        public string? ListOrder { get; set; } // thứ tự của các list
        public string Slug { get; set; } = null!;
        public int WorkspaceId { get; set; }
        public Workspace? Workspace { get; set; }
        public List<Assignment> Assignments { get; set; } = [];
        public List<ProjectInvitation> ProjectInvitations { get; set; } = [];
        public List<List> Lists { get; set; } = [];
        public List<ProjectRequest> Requests { get; set; } = [];
        public List<Notification> Notifications { get; set; } = [];
        public List<ChangeLog> ChangeLogs { get; set; } = [];
        public List<ProjectComment> Comments { get; set; } = [];
    }
}