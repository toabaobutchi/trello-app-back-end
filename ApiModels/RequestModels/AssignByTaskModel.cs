namespace backend_apis.ApiModels.RequestModels
{
    public sealed record AssignByTaskModel
    {
        public IEnumerable<string> AssignmentIds { get; set; } = [];
    }
}