namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record RelatedTaskResponse
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string ListId { get; set; } = null!;
        public bool? IsCompleted { get; set; }
        public bool? IsReOpened { get; set; }
        public bool? IsMarkedNeedHelp { get; set; }
        public long? DueDate { get; set; }
        public string? Priority { get; set; }
        public long CreatedAt { get; set; }
        public long? StartedAt { get; set; }

        public static RelatedTaskResponse Create(Models.Task task)
        {
            return new RelatedTaskResponse
            {
                Id = task.Id.ToString(),
                Name = task.Name,
                ListId = task.ListId ?? "",
                IsCompleted = task.IsCompleted,
                IsReOpened = task.IsReOpened,
                IsMarkedNeedHelp = task.IsMarkedNeedHelp,
                DueDate = task.DueDate,
                Priority = task.Priority.ToString(),
                CreatedAt = task.CreatedAt,
                StartedAt = task.StartedAt
            };
        }
    }
}