using System.ComponentModel.DataAnnotations.Schema;

namespace backend_apis.Models
{
    public class Task
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public long CreatedAt { get; set; }
        public long? StartedAt { get; set; }
        public EPriority? Priority { get; set; }
        public long? LastListUpdatedAt { get; set; }
        public long? DueDate { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? IsMarkedNeedHelp { get; set; }
        public bool? IsReOpened { get; set; }
        public long? StatusChangeAt { get; set; }
        public bool? IsDeleted { get; set; }
        public long? DeletedAt { get; set; }
        public string? DeleterId { get; set; }
        public Assignment? Deleter { get; set; }

        [ForeignKey(nameof(Creator))]
        public string CreatorId { get; set; } = null!;
        public string? ListId { get; set; }
        public List? List { get; set; }
        public Assignment Creator { get; set; } = null!;
        public List<Attachment> Attachments { get; set; } = [];
        public List<Comment> Comments { get; set; } = [];
        public List<TaskAssignment> TaskAssignments { get; set; } = [];
        public List<SubTask> SubTasks { get; set; } = [];
        public List<TaskDependenceDetail> ParentDependentTasks { get; set; } = [];
        public List<TaskDependenceDetail> ChildDependentTasks { get; set; } = [];
    }
}