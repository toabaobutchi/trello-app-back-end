using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseForBoardDisplay_v3
{
    public sealed record ListResponseForBoardDisplay_v3
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public long CreatedAt { get; set; }
        public string? TaskOrder { get; set; }
        public string ProjectId { get; set; } = null!;
        public int? WipLimit { get; set; }
        public List<TaskResponseForBoardDisplay_v3> Tasks { get; set; } = [];

        public static ListResponseForBoardDisplay_v3 Create(List list)
        {
            return new ListResponseForBoardDisplay_v3
            {
                Id = list.Id,
                Name = list.Name,
                ProjectId = list.ProjectId,
                CreatedAt = list.CreatedAt,
                TaskOrder = list.TaskOrder,
                WipLimit = list.WipLimit
            };
        }
        public ListResponseForBoardDisplay_v3 SetTasks(IEnumerable<TaskResponseForBoardDisplay_v3> tasks)
        {
            Tasks.AddRange(tasks);
            return this;
        }
    }
}