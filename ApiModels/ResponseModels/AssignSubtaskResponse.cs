namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record AssignSubtaskResponse : JoinSubtaskResponse
    {
        public string? AssignerId { get; set; }
        public bool IsNewAssignment { get; set; }

        public static new AssignSubtaskResponse Create(Models.SubTask subtask, bool isNewAssignment = false)
        {
            return new AssignSubtaskResponse
            {
                Id = subtask.Id,
                Title = subtask.Title,
                TaskId = subtask.TaskId.ToString(),
                IsCompleted = subtask.IsCompleted,
                CompletedAt = subtask.CompletedAt,
                AssignmentId = subtask.AssignmentId,
                AssignedAt = subtask.AssignedAt,
                AssignerId = subtask.AssignerId,
                IsNewAssignment = isNewAssignment
            };
        }
    }
}