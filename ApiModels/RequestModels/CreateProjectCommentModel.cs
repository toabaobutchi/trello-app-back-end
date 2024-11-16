namespace backend_apis.ApiModels.RequestModels
{
    public sealed record CreateProjectCommentModel
    {
        public string Content { get; set; } = null!;
    }
}