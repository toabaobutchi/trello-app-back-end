namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record UpdatedTaskResponse
    {
        public string Id { get; set; } = null!;
        public string ListId { get; set; } = null!;
        public string? Name { get; set; }
        public long? StartedAt { get; set; }
        public long? DueDate { get; set; }
        public string? Priority { get; set; }
        public string? Description { get; set; }

        public static UpdatedTaskResponse Create(Models.Task task)
        {
            return new UpdatedTaskResponse()
            {
                Id = task.Id.ToString(),
                ListId = task.ListId ?? "",
                Name = task.Name,
                DueDate = task.DueDate,
                Priority = task.Priority.ToString(),
                Description = task.Description,
                StartedAt = task.StartedAt
            };
        }
    }
}