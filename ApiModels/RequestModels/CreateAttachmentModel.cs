using backend_apis.Models;
using backend_apis.Utils;

namespace backend_apis.ApiModels.RequestModels
{
    public sealed record CreateAttachmentModel
    {
        public string Link { get; set; } = null!;
        public string DisplayText { get; set; } = null!;
        public string TaskId { get; set; } = null!;
        public string AssignmentId { get; set; } = null!;

        public Attachment ToAttachment()
        {
            return new Attachment()
            {
                Link = Link,
                DisplayText = DisplayText,
                TaskId = Guid.Parse(TaskId),
                AssignmentId = AssignmentId,
                CreatedAt = DateTimeUtils.GetSeconds(),
            };
        }
    }
}