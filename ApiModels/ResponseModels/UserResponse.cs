namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record UserResponse
    {
        public string Id { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string Email { get; set; } = null!;
        public string? Avatar { get; set; }

        public static UserResponse Create(Models.User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Avatar = user.Avatar
            };
        }
    }
}