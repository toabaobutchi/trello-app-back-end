namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record ProjectCommentResponse
    {
        public string Id { get; set; } = null!;
        public string Content { get; set; } = null!;
        public long CommentAt { get; set; }
        public string ProjectId { get; set; } = null!;
        public string? AssignmentId { get; set; }

        public static ProjectCommentResponse Create(Models.ProjectComment comment)
        {
            return new ProjectCommentResponse
            {
                Id = comment.Id,
                Content = comment.Content,
                CommentAt = comment.CommentAt,
                ProjectId = comment.ProjectId,
                AssignmentId = comment.AssignmentId
            };
        }
    }
}