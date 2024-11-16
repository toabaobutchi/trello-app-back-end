namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record MarkedTaskResponse
    {
        public string Id { get; set; } = null!;
        public string ListId { get; set; } = null!;
        public bool? IsMarkedNeedHelp { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? IsReOpened { get; set; }

        public static MarkedTaskResponse Create(in Models.Task task)
        {
            return new MarkedTaskResponse
            {
                Id = task.Id.ToString(),
                ListId = task.ListId ?? "",
                IsMarkedNeedHelp = task.IsMarkedNeedHelp,
                IsCompleted = task.IsCompleted,
                IsReOpened = task.IsReOpened
            };
        }
    }
}