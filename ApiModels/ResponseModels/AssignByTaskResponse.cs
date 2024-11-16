namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record AssignByTaskResponse
    {
        public string TaskId { get; set; } = null!;
        public IEnumerable<string> AssignmentIds { get; set; } = [];
        public string? AssignerId { get; set; }

        public static AssignByTaskResponse Create(string taskId, IEnumerable<string> assignmentIds, string assignerId)
        {
            return new AssignByTaskResponse
            {
                TaskId = taskId,
                AssignmentIds = assignmentIds,
                AssignerId = assignerId
            };
        }
    }
}