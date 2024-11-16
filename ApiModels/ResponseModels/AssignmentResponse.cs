using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record AssignmentResponse
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string Email { get; set; } = null!;
        public string ProjectId { get; set; } = null!;
        public string? Avatar { get; set; }
        public string? Permission { get; set; }

        public static implicit operator AssignmentResponse(Assignment assignment)
        {
            return new AssignmentResponse()
            {
                Id = assignment.Id,
                DisplayName = assignment.User.DisplayName,
                Email = assignment.User?.Email ?? "",
                Avatar = assignment.User?.Avatar,
                Permission = assignment.Permission.ToString(),
                UserId = assignment.UserId,
                ProjectId = assignment.ProjectId
            };
        }
    }
}