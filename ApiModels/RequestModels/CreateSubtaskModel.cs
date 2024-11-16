using backend_apis.Models;
using backend_apis.Utils;

namespace backend_apis.ApiModels.RequestModels
{
    public class CreateSubtaskModel
    {
        public IEnumerable<string> Names { get; set; } = null!;
        public string TaskId { get; set; } = null!;

        public IEnumerable<SubTask> ToSubtasks()
        {
            return Names.Select(name => new SubTask()
            {
                Title = name,
                TaskId = Guid.Parse(TaskId),
                CreatedAt = DateTimeUtils.GetSeconds()
            });
        }
    }
}