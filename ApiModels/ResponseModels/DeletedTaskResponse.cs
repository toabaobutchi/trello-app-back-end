namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record DeletedTaskResponse
    {
        public string Id { get; set; } = null!;
        public string ListId { get; set; } = null!;
    }
}