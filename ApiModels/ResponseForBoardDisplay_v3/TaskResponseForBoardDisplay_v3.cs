using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseForBoardDisplay_v3
{
    public sealed record TaskResponseForBoardDisplay_v3
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; }
        public long CreatedAt { get; set; }
        public long? StartedAt { get; set; }
        public long? DueDate { get; set; }
        public string? Priority { get; set; } = null!;
        public IEnumerable<string> TaskAssignmentIds { get; set; } = [];
        public int CommentCount { get; set; }
        public int CompletedSubTaskCount { get; set; }
        public bool? isReOpened { get; set; }
        public int SubTaskCount { get; set; }
        public bool? IsMarkedNeedHelp { get; set; }
        public string? ListId { get; set; }
        public IEnumerable<string> DependencyIds { get; set; } = [];
        public static TaskResponseForBoardDisplay_v3 Create(Models.Task task, IEnumerable<TaskAssignment> taskAssignments, IEnumerable<SubTask> subTasks, IEnumerable<Comment> comments, IEnumerable<TaskDependenceDetail> parentTasksDependenceDetails)
        {
            return new TaskResponseForBoardDisplay_v3
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority.ToString(),
                TaskAssignmentIds = taskAssignments.Select(t => t.AssignmentId),
                CompletedSubTaskCount = subTasks.Count(s => s.IsCompleted),
                SubTaskCount = subTasks.Count(),
                ListId = task.ListId,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt,
                IsMarkedNeedHelp = task.IsMarkedNeedHelp,
                CommentCount = comments.Count(),
                isReOpened = task.IsReOpened,
                StartedAt = task.StartedAt,
                DependencyIds = parentTasksDependenceDetails.Select(t => t.DependentTaskId.ToString())
            };
        }
    }
    // public sealed record TaskDependenciesResponseForBoardDisplay_v3
    // {
    //     public string Id { get; set; } = null!;
    //     public string Name { get; set; } = null!;
    //     public string ListId { get; set; } = null!;

    //     public static TaskDependenciesResponseForBoardDisplay_v3 Create(Models.Task task)
    //     {
    //         return new TaskDependenciesResponseForBoardDisplay_v3
    //         {
    //             Id = task.Id.ToString(),
    //             Name = task.Name,
    //             ListId = task.ListId
    //         };
    //     }
    // }
}