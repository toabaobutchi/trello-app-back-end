namespace backend_apis.ApiModels.RequestModels
{
    public sealed record AddChildrenTaskModel
    {
        public IEnumerable<string> Children { get; set; } = [];
    }
}