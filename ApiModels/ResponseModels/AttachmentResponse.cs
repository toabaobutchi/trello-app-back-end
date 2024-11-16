using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseModels
{
    public class AttachmentResponse
    {
        public int Id { get; set; }
        public string Link { get; set; } = null!;
        public string? DisplayText { get; set; }
        public long CreatedAt { get; set; }
        public string TaskId { get; set; } = null!;
        public string? AssignmentId { get; set; } = null!;

        public static AttachmentResponse Create(Attachment attachment)
        {
            return new AttachmentResponse
            {
                Id = attachment.Id,
                Link = attachment.Link,
                DisplayText = attachment.DisplayText,
                CreatedAt = attachment.CreatedAt,
                TaskId = attachment.TaskId.ToString(),
                AssignmentId = attachment.AssignmentId
            };
        }
    }
}