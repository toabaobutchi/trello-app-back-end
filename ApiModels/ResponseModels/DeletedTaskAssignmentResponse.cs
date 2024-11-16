namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record DeletedTaskAssignmentResponse
    {
        public string TaskId { get; set; } = null!;
        public string AssignmentId { get; set; } = null!;

        public static DeletedTaskAssignmentResponse Create(Models.TaskAssignment taskAssignment)
        {
            return new DeletedTaskAssignmentResponse()
            {
                TaskId = taskAssignment.TaskId.ToString(),
                AssignmentId = taskAssignment.AssignmentId
            };
        }
    }
}