using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseForTableDisplay
{
    public class TaskResponseForTableDisplay
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool? IsCompleted { get; set; }
        public long CreatedAt { get; set; }
        public long? DueDate { get; set; }
        public string? Priority { get; set; }
        public List<string> TaskAssignmentIds { get; set; } = [];
        public string ListId { get; set; } = null!;
        public string ListName { get; set; } = null!;
        public bool? IsMarkedNeedHelp { get; set; }
        public string? CreatorId { get; set; }

        public static TaskResponseForTableDisplay Create(Models.Task task, string listName, IEnumerable<TaskAssignment> taskAssignment)
        {
            return new TaskResponseForTableDisplay
            {
                Id = task.Id.ToString(),
                Name = task.Name,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate,
                Priority = task.Priority?.ToString(),
                TaskAssignmentIds = taskAssignment.Select(t => t.AssignerId).ToList(),
                ListId = task.ListId,
                ListName = listName,
                IsMarkedNeedHelp = task.IsMarkedNeedHelp,
                CreatorId = task.CreatorId,
            };
        }
    }
}