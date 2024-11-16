using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseForBoardDisplay_v3
{
    public class ProjectResponseForBoardDisplay_v3 : ContextBase
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Color { get; set; }
        public long CreatedAt { get; set; }
        public string Slug { get; set; } = null!;
        public long? DueDate { get; set; }
        public int WorkspaceId { get; set; }
        public string? ListOrder { get; set; }
        public string AssignmentId { get; set; } = null!;
        public List<ListResponseForBoardDisplay_v3> Lists { get; set; } = [];

        public static ProjectResponseForBoardDisplay_v3 Create(Project project, string context)
        {
            return new ProjectResponseForBoardDisplay_v3
            {
                Id = project.Id,
                Name = project.Name,
                Color = project.Color,
                CreatedAt = project.CreatedAt,
                DueDate = project.DueDate,
                WorkspaceId = project.WorkspaceId,
                Context = context,
                Slug = project.Slug,
                ListOrder = project.ListOrder
            };
        }
        public ProjectResponseForBoardDisplay_v3 SetLists(List<List> list)
        {
            Lists = list.Select(ListResponseForBoardDisplay_v3.Create).ToList();
            return this;
        }
    }
}