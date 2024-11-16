using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record UpdateProjectResponse
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public long? DueDate { get; set; }
        public string? Color { get; set; }

        public static UpdateProjectResponse Create(Project project)
        {
            return new UpdateProjectResponse()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                DueDate = project.DueDate,
                Color = project.Color
            };
        }
    }
}