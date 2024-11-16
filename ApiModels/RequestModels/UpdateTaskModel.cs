using backend_apis.Models;

namespace backend_apis.ApiModels.RequestModels
{
    public sealed record UpdateTaskModel
    {
        public string? Name { get; set; }
        public long? DueDate { get; set; }
        public string? Priority { get; set; }
        public string? Description { get; set; }
        public long? StartedAt { get; set; }

        public Models.Task? Update(ref Models.Task task)
        {
            var priortity = Enum.TryParse(Priority, out EPriority ePriority);
            if (priortity)
            {
                task.Priority = ePriority;
            }
            task.Name = Name ?? task.Name;
            task.DueDate = DueDate ?? task.DueDate;
            task.Description = Description ?? task.Description;
            task.StartedAt = StartedAt ?? task.StartedAt;
            return task;
        }
    }
}