using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseForBoardDisplay
{
    public class ListResponseForBoardDisplay
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public long CreatedAt { get; set; }
        public string? TaskOrder { get; set; }
        public string ProjectId { get; set; } = null!;
        public List<TaskResponseForBoardDisplay> Tasks { get; set; } = [];

        public static ListResponseForBoardDisplay Create(List list)
        {
            return new ListResponseForBoardDisplay
            {
                Id = list.Id,
                Name = list.Name,
                ProjectId = list.ProjectId,
                CreatedAt = list.CreatedAt,
                TaskOrder = list.TaskOrder,
            };
        }
        public ListResponseForBoardDisplay SetTasks(IEnumerable<TaskResponseForBoardDisplay> tasks) {
            Tasks.AddRange(tasks);
            return this;
        }
    }
}