using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseModels
{
    public class WorkspaceResponse : ContextBase
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Slug { get; set; } = null!;
        public long CreatedAt { get; set; }
        public string OwnerId { get; set; } = null!;

        public static WorkspaceResponse Create(Workspace workspace, string context)
        {
            return new WorkspaceResponse
            {
                Id = workspace.Id,
                Name = workspace.Name,
                Description = workspace.Description,
                Slug = workspace.Slug,
                CreatedAt = workspace.CreatedAt,
                OwnerId = workspace.OwnerId,
                Context = context
            };
        }
    }
}