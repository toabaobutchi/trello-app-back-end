using backend_apis.Models;
using backend_apis.Utils;

namespace backend_apis.ApiModels.RequestModels
{
    public sealed record CreateCommentModel
    {
        public string Content { get; set; } = null!;
        public string TaskId { get; set; } = null!;
        public string AssignmentId { get; set; } = null!;

        public Comment ToComment()
        {
            return new Comment
            {
                Content = Content,
                TaskId = Guid.Parse(TaskId),
                AssignmentId = AssignmentId,
                CommentAt = DateTimeUtils.GetSeconds()
            };
        }
    }
}