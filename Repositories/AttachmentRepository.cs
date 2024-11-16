
using backend_apis.ApiModels.RequestModels;
using backend_apis.Data;
using backend_apis.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Repositories
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly ProjectManagerDbContext _db;

        public AttachmentRepository(ProjectManagerDbContext db)
        {
            _db = db;
        }
        public async Task<Attachment?> CreateAttachmentAsync(CreateAttachmentModel model)
        {
            try
            {
                var attachment = model.ToAttachment();
                await _db.Attachments.AddAsync(attachment);
                await _db.SaveChangesAsync();
                return attachment;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Attachment>?> GetAttachmentsAsync(string taskId)
        {
            try
            {
                var parseResult = Guid.TryParse(taskId, out var validId);
                if (!parseResult)
                {
                    return null;
                }
                var attachments = await _db.Attachments.AsNoTracking().Where(a => a.TaskId == validId).ToListAsync();
                return attachments;
            }
            catch
            {
                return null;
            }
        }
    }
}