namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record ResetTaskResponse
    {
        public string Id { get; set; } = null!;
        public string ListId { get; set; } = null!;
        public long? DueDate { get; set; }
        public long? StartedAt { get; set; }
        public string? Priority { get; set; }
        public string? Description { get; set; }

        public static ResetTaskResponse Create(Models.Task task)
        {
            return new ResetTaskResponse()
            {
                Id = task.Id.ToString(),
                ListId = task.ListId,
                DueDate = task.DueDate,
                StartedAt = task.StartedAt,
                Priority = task.Priority.ToString(),
                Description = task.Description,
            };
        }
    }
}