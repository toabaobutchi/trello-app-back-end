namespace backend_apis.ApiModels.ResponseForBoardDisplay
{
    public class TaskResponseForBoardDisplay
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; }
        public long CreatedAt { get; set; }
        public long? DueDate { get; set; }
        public string? Priority { get; set; } = null!;
        public int AssigneeCount { get; set; }
        public long? LastListUpdatedAt { get; set; }
        public int CompletedSubTaskCount { get; set; }
        public int SubTaskCount { get; set; }
        public bool? IsMarkedNeedHelp { get; set; }
        public string? ListId { get; set; }
        public static TaskResponseForBoardDisplay Create(Models.Task task)
        {
            return new TaskResponseForBoardDisplay
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority.ToString(),
                AssigneeCount = task.TaskAssignments.Count(),
                CompletedSubTaskCount = task.SubTasks.Count(s => s.IsCompleted),
                SubTaskCount = task.SubTasks.Count,
                ListId = task.ListId,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt,
                IsMarkedNeedHelp = task.IsMarkedNeedHelp,
                LastListUpdatedAt = task.LastListUpdatedAt
            };
        }
    }
}