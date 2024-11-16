namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record JoinTaskResponse
    {
        public string TaskId { get; set; } = null!;
        public string AssignmentId { get; set; } = null!;
        public long AssignedAt { get; set; }
        public string? AssignerId { get; set; }

        public static JoinTaskResponse Create(Models.TaskAssignment taskAssignment)
        {
            return new JoinTaskResponse()
            {
                TaskId = taskAssignment.TaskId.ToString(),
                AssignmentId = taskAssignment.AssignmentId,
                AssignedAt = taskAssignment.AssignedAt,
                AssignerId = taskAssignment.AssignerId
            };
        }
    }
}