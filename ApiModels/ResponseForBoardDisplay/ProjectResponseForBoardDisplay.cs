using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseForBoardDisplay
{
    // dùng để chứa cá thông tin sẽ phản hồi cho client khi yêu cầu dữ liệu để hiển thị dạng bảng (board)
    public class ProjectResponseForBoardDisplay : ContextBase
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Color { get; set; }
        public long CreatedAt { get; set; }
        public string Slug { get; set; } = null!;
        public long? DueDate { get; set; }
        public int WorkspaceId { get; set; }
        public string? ListOrder { get; set; }
        public int MemberCount { get; set; }
        public List<ListResponseForBoardDisplay> Lists { get; set; } = [];

        public static ProjectResponseForBoardDisplay Create(Project project, int memberCount, string context)
        {
            return new ProjectResponseForBoardDisplay
            {
                Id = project.Id,
                Name = project.Name,
                Color = project.Color,
                CreatedAt = project.CreatedAt,
                DueDate = project.DueDate,
                WorkspaceId = project.WorkspaceId,
                Context = context,
                MemberCount = memberCount,
                Slug = project.Slug,
                ListOrder = project.ListOrder
            };
        }
        public ProjectResponseForBoardDisplay SetLists(List<List> list)
        {
            Lists = list.Select(l => ListResponseForBoardDisplay.Create(l)).ToList();
            return this;
        }
    }
}