namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record SubtaskResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public long CreatedAt { get; set; }
        public string TaskId { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public long? CompletedAt { get; set; }
        public string? AssignmentId { get; set; }
        public static SubtaskResponse Create(Models.SubTask subtask)
        {
            return new SubtaskResponse()
            {
                Id = subtask.Id,
                Title = subtask.Title,
                CreatedAt = subtask.CreatedAt,
                TaskId = subtask.TaskId.ToString(),
                IsCompleted = subtask.IsCompleted,
                CompletedAt = subtask.CompletedAt,
                AssignmentId = subtask.AssignmentId
            };
        }
    }
}