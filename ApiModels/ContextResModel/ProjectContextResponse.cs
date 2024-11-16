using backend_apis.Models;

namespace backend_apis.ApiModels.ContextResModel
{
    public class ProjectContextResponse : ContextBase
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public long CreatedAt { get; set; }
        public long? DueDate { get; set; }
        public string? Color { get; set; }
        public string Slug { get; set; } = null!;
        public int WorkspaceId { get; set; }

        public static ProjectContextResponse Create(Project project, string context) {
            return new ProjectContextResponse
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                DueDate = project.DueDate,
                Color = project.Color,
                Slug = project.Slug,
                WorkspaceId = project.WorkspaceId,
                Context = context
            };
        }
    }
}