namespace backend_apis.ApiModels
{
    public sealed record MemberConnection
    {
        public string? ConnectionId { get; set; }
        public string UserId { get; set; } = null!;
        public string ProjectId { get; set; } = null!;
    }
}