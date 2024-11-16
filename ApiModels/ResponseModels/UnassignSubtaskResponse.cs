namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record UnassignSubtaskResponse
    {
        public int SubtaskId { get; set; }
        public string TaskId { get; set; } = null!;
        public string? AssignmentId { get; set; }

        public static UnassignSubtaskResponse Create(Models.SubTask subtask)
        {
            return new UnassignSubtaskResponse()
            {
                SubtaskId = subtask.Id,
                TaskId = subtask.TaskId.ToString(),
                AssignmentId = subtask.AssignmentId
            };
        }
    }
}