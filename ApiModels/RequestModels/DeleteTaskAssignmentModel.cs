namespace backend_apis.ApiModels.RequestModels
{
    public sealed record DeleteTaskAssignmentModel
    {
        public string AssignmentId { get; set; } = null!;
    }
}