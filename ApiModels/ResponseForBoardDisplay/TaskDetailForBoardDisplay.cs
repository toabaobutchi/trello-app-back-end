using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseForBoardDisplay
{
    public sealed record TaskDetailForBoardDisplay
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool? IsMarkedNeedHelp { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? IsReOpened { get; set; }
        public long CreatedAt { get; set; }
        public long? StartedAt { get; set; }
        public string Priority { get; set; } = null!;
        public string? Description { get; set; }
        public string ListId { get; set; } = null!;
        public string ListName { get; set; } = null!;
        public long? DueDate { get; set; }
        public int AttachmentsCount { get; set; }
        public string? CreatorId { get; set; }
        public long? LastListUpdatedAt { get; set; }
        public IEnumerable<string> TaskAssignmentIds { get; set; } = [];
        public IEnumerable<SubtaskForBoard>? SubTasks { get; set; } = [];

        public static TaskDetailForBoardDisplay Create(Models.Task task, string listName, int attachmentCount, IEnumerable<string> taskAssignmentIds, IEnumerable<SubTask> subtasks)
        {
            return new TaskDetailForBoardDisplay
            {
                Id = task.Id.ToString(),
                Name = task.Name,
                IsMarkedNeedHelp = task.IsMarkedNeedHelp,
                Priority = task.Priority?.ToString() ?? "",
                Description = task.Description,
                ListId = task.ListId,
                ListName = listName,
                DueDate = task.DueDate,
                AttachmentsCount = attachmentCount,
                CreatorId = task.CreatorId,
                TaskAssignmentIds = taskAssignmentIds,
                SubTasks = subtasks?.Select(s => SubtaskForBoard.Create(s)) ?? Enumerable.Empty<SubtaskForBoard>(),
                IsCompleted = task.IsCompleted,
                IsReOpened = task.IsReOpened,
                LastListUpdatedAt = task.LastListUpdatedAt,
                StartedAt = task.StartedAt,
                CreatedAt = task.CreatedAt,
            };
        }
    }
}