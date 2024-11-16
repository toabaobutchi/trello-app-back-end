namespace backend_apis.ApiModels.RequestModels
{
    public sealed record UnassignSubtaskModel
    {
        public string AssignmentId { get; set; } = null!;
    }
}