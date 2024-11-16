using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record ProjectResponseForUpdating
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public long? DueDate { get; set; }
        public string? Color { get; set; }
        public long? MinimunAllowedDueDate { get; set; }

        public static ProjectResponseForUpdating Create(Project project, long? minimunAllowedDueDate = null)
        {
            return new ProjectResponseForUpdating()
            {
                Name = project.Name,
                Description = project.Description,
                DueDate = project.DueDate,
                Color = project.Color,
                MinimunAllowedDueDate = minimunAllowedDueDate
            };
        }
    }
}