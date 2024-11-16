using backend_apis.ApiModels.RequestModels;
using backend_apis.Models;

namespace backend_apis.Repositories
{
    public interface IAttachmentRepository
    {
        Task<Attachment?> CreateAttachmentAsync(CreateAttachmentModel model);
        Task<List<Attachment>?> GetAttachmentsAsync(string taskId);
    }
}