using Microsoft.EntityFrameworkCore;

namespace backend_apis.Models
{
    public class Assignment
    {
        public string Id { get; set; } = null!;
        public long JoinAt { get; set; }
        public long? LastViewAt { get; set; }
        public EPermission Permission { get; set; }
        public bool? isPinned { get; set; }
        public string ProjectId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public Project Project { get; set; } = null!;
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public User User { get; set; } = null!;
        public List<Comment> Comments { get; set; } = [];
        public List<TaskAssignment> TaskAssignments { get; set; } = [];
        public List<TaskAssignment> TaskAssigners { get; set; } = [];
        public List<SubTask> SubTasks { get; set; } = [];
        public List<SubTask> AssignSubtasks { get; set; } = [];
        public List<Attachment> Attachments { get; set; } = [];
        public List<Task> CreateTasks { get; set; } = [];
        public List<ProjectInvitation> ProjectInvitations { get; set; } = [];
        public List<ChangeLog> ChangeLogs { get; set; } = [];
        public List<Task> DeleteTasks { get; set; } = [];
        public List<ProjectComment> ProjectComments { get; set; } = [];

    }
}