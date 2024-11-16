namespace backend_apis.ApiModels.RequestModels
{
    public sealed record AssignSubtaskModel
    {
        public string AssignmentId { get; set; } = null!;
    }
}