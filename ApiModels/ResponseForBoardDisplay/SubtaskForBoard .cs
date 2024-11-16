namespace backend_apis.ApiModels.ResponseForBoardDisplay
{
    public sealed record SubtaskForBoard
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string TaskId { get; set; } = null!;
        public bool? IsCompleted { get; set; }
        public string? AssignmentId { get; set; }
        public static SubtaskForBoard Create(Models.SubTask subtask)
        {
            return new SubtaskForBoard
            {
                Id = subtask.Id,
                Title = subtask.Title,
                TaskId = subtask.TaskId.ToString(),
                IsCompleted = subtask.IsCompleted,
                AssignmentId = subtask.AssignmentId,
            };
        }
    }
}