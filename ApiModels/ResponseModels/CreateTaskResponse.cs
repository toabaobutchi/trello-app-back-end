using backend_apis.ApiModels.ResponseForBoardDisplay;

namespace backend_apis.ApiModels.ResponseModels
{
    public class CreateTaskResponse
    {
        public TaskResponseForBoardDisplay CreatedTask { get; set; } = null!;
        public string? TaskOrder { get; set; }
        public static CreateTaskResponse Create(Models.Task task, string? taskOrder = "") {
            return new CreateTaskResponse() {
                CreatedTask = TaskResponseForBoardDisplay.Create(task),
                TaskOrder = taskOrder,
            };
        }
    }
}