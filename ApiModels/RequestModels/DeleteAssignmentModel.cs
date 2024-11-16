namespace backend_apis.ApiModels.RequestModels
{
    public sealed record DeleteAssignmentModel
    {
        public string AssignmentId { get; set; } = null!;
    }
}