using backend_apis.Models;
using backend_apis.Utils;

namespace backend_apis.ApiModels.RequestModels
{
    public class CreateWorkspaceModel
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public Workspace ToWorkspace()
        {
            string slug = TextUtils.CreateSlug(Name);
            long createdAt = DateTimeUtils.GetSeconds();
            return new Workspace
            {
                Name = Name,
                Description = Description,
                CreatedAt = createdAt,
                Slug = $"{slug}-{createdAt}"
            };
        }
    }
}