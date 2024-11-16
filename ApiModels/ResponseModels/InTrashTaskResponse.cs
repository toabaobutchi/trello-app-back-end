namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record InTrashTaskResponse
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public long DeletedAt { get; set; }

        public DeleterResponse? Deleter { get; set; }

        public static InTrashTaskResponse Create(Models.Task task, Models.User? user)
        {
            return new InTrashTaskResponse()
            {
                Id = task.Id.ToString(),
                Name = task.Name,
                Description = task.Description,
                DeletedAt = task.DeletedAt.Value,
                Deleter = user is not null ? new DeleterResponse()
                {
                    Email = user.Email,
                    DisplayName = user.DisplayName ?? "",
                    Avatar = user.Avatar,
                    Id = task.DeleterId ?? ""
                } : null
            };
        }
    }
    public sealed record DeleterResponse
    {
        public string Email { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string? Avatar { get; set; }
        public string Id { get; set; } = null!;
    }
}