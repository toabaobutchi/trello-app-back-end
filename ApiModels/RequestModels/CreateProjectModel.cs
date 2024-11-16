using System.ComponentModel.DataAnnotations;
using backend_apis.Models;
using backend_apis.Utils;

namespace backend_apis.ApiModels.RequestModels
{
    public class CreateProjectModel
    {
        [Required]
        public string Name { get; set; } = null!;
        public string? Color { get; set; }
        public int WorkspaceId { get; set; }
        public string? Description { get; set; }
        public long? DueDate { get; set; }
        public string? ListOrder { get; set; }
        public Project ToProject()
        {
            var slug = TextUtils.CreateSlug(Name);
            var createdAt = DateTimeUtils.GetSeconds();
            return new Project()
            {
                Id = TextUtils.CreateId(),
                Description = Description,
                Name = Name,
                Color = Color,
                CreatedAt = createdAt,
                DueDate = DueDate,
                Slug = $"{slug}-{createdAt}",
                WorkspaceId = WorkspaceId,
                ListOrder = ListOrder
            };
        }
    }
}