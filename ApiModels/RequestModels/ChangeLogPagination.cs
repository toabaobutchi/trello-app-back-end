namespace backend_apis.ApiModels.RequestModels
{
    public sealed record ChangeLogPagination
    {
        public int? Offset { get; set; }
        public int? Count { get; set; }
        public long? Date { get; set; }
        public string? Uid { get; set; }
        public string? Task { get; set; }
    }
}