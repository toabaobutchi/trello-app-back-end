namespace backend_apis.ApiModels.ResponseModels
{
    public sealed class ProjectResponse : ContextBase
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public long CreatedAt { get; set; }
        public long? DueDate { get; set; }
        public string? Color { get; set; }
        public string? ListOrder { get; set; } // thứ tự của các list
        public string Slug { get; set; } = null!;
        public int WorkspaceId { get; set; }
        public static ProjectResponse Create(in Models.Project project, string context)
        {
            return new ProjectResponse()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                DueDate = project.DueDate,
                Color = project.Color,
                ListOrder = project.ListOrder,
                Slug = project.Slug,
                WorkspaceId = project.WorkspaceId,
                Context = context
            };
        }
    }
}