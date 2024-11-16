namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record CommentResponse
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public long CommentAt { get; set; }
        public string TaskId { get; set; } = null!;
        public string AssignmentId { get; set; } = null!;

        public static CommentResponse Create(Models.Comment comment)
        {
            return new CommentResponse
            {
                Id = comment.Id,
                Content = comment.Content,
                CommentAt = comment.CommentAt,
                TaskId = comment.TaskId.ToString(),
                AssignmentId = comment.AssignmentId
            };
        }
    }
}