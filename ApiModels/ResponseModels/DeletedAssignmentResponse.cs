namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record DeletedAssignmentResponse
    {
        public string AssignmentId { get; set; } = null!;
    }
}