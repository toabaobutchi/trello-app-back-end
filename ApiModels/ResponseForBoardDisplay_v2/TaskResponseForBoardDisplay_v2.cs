namespace backend_apis.ApiModels.ResponseForBoardDisplay_v2
{
    public sealed record TaskResponseForBoardDisplay_v2
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; }
        public long CreatedAt { get; set; }
        public long? StartedAt { get; set; }
        public long? DueDate { get; set; }
        public string? Priority { get; set; } = null!;
        // public int AssigneeCount { get; set; }
        public IEnumerable<string> TaskAssignmentIds { get; set; } = [];
        public int CommentCount { get; set; }
        public int CompletedSubTaskCount { get; set; }
        public bool? isReOpened { get; set; }
        public int SubTaskCount { get; set; }
        public bool? IsMarkedNeedHelp { get; set; }
        public string? ListId { get; set; }
        public static TaskResponseForBoardDisplay_v2 Create(Models.Task task)
        {
            return new TaskResponseForBoardDisplay_v2
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority.ToString(),
                TaskAssignmentIds = task.TaskAssignments.Select(t => t.AssignmentId),
                CompletedSubTaskCount = task.SubTasks.Count(s => s.IsCompleted),
                SubTaskCount = task.SubTasks.Count,
                ListId = task.ListId,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt,
                IsMarkedNeedHelp = task.IsMarkedNeedHelp,
                CommentCount = task.Comments.Count,
                isReOpened = task.IsReOpened,
                StartedAt = task.StartedAt,
            };
        }
    }
}