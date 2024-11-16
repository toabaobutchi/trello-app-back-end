namespace backend_apis.ApiModels.ResponseModels
{
    public record JoinSubtaskResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string TaskId { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public long? CompletedAt { get; set; }
        public string? AssignmentId { get; set; }
        public long? AssignedAt { get; set; }

        public static JoinSubtaskResponse Create(Models.SubTask joinSubtask)
        {
            return new JoinSubtaskResponse
            {
                Id = joinSubtask.Id,
                Title = joinSubtask.Title,
                TaskId = joinSubtask.TaskId.ToString(),
                IsCompleted = joinSubtask.IsCompleted,
                CompletedAt = joinSubtask.CompletedAt,
                AssignmentId = joinSubtask.AssignmentId,
                AssignedAt = joinSubtask.AssignedAt
            };
        }
    }
}