using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseForTableDisplay
{
    public class ProjectResponseForTableDisplay : ContextBase
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public long CreatedAt { get; set; }
        public long? DueDate { get; set; }
        public string? Color { get; set; }
        public List<TaskResponseForTableDisplay> Tasks { get; set; } = [];

        public static ProjectResponseForTableDisplay Create(Project project, string context)
        {
            return new ProjectResponseForTableDisplay
            {
                Id = project.Id,
                Name = project.Name,
                CreatedAt = project.CreatedAt,
                DueDate = project.DueDate,
                Color = project.Color,
                Context = context
            };
        }

    }
}