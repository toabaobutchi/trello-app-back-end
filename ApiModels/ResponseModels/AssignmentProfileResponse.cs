namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record AssignmentProfileResponse
    {
        public string Id { get; set; } = null!;
        public long JoinAt { get; set; }
        public IEnumerable<JoinedTaskResponse> JoinedTasks { get; set; } = [];
        public static AssignmentProfileResponse Create(Models.Assignment assignment)
        {
            return new AssignmentProfileResponse
            {
                Id = assignment.Id,
                JoinAt = assignment.JoinAt,
            };
        }
    }
    public sealed record JoinedTaskResponse
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool? IsCompleted { get; set; }
        public long? DueDate { get; set; } = null!;
        public string? Priority { get; set; } = null!;
        public string ListId { get; set; } = null!;
        public string ListName { get; set; } = null!;
        public bool? IsMarkedNeedHelp { get; set; } = null!;
        public long AssignedAt { get; set; }
        public int AssignmentCount { get; set; }
        public static JoinedTaskResponse Create(Models.Task task, int assignmentCount, string listName, long assignedAt)
        {
            return new JoinedTaskResponse
            {
                Id = task.Id.ToString(),
                Name = task.Name,
                IsCompleted = task.IsCompleted,
                DueDate = task.DueDate,
                AssignmentCount = assignmentCount,
                Priority = task.Priority?.ToString(),
                ListId = task.ListId,
                ListName = listName,
                IsMarkedNeedHelp = task.IsMarkedNeedHelp,
                AssignedAt = assignedAt
            };
        }
    }
}