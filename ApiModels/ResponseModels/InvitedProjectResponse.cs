using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record InvitedProjectResponse
    {
        public string Id { get; set; } = null!;
        public string InvitationId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public long CreatedAt { get; set; }
        public long? DueDate { get; set; }
        public string Slug { get; set; } = null!;
        public string? Color { get; set; }
        public string? Description { get; set; }
        public string InvitedPermission { get; set; } = null!;
        public string InviterEmail { get; set; } = null!;
        public long InvitedAt { get; set; }
        public static InvitedProjectResponse Create(ProjectInvitation invitation, Project project, string inviterEmail)
        {
            return new InvitedProjectResponse
            {
                Id = project.Id,
                Name = project.Name,
                CreatedAt = project.CreatedAt,
                DueDate = project.DueDate,
                Color = project.Color,
                Description = project.Description,
                InvitedPermission = invitation.Permission.ToString(),
                InviterEmail = inviterEmail,
                InvitedAt = invitation.InvitedAt,
                InvitationId = invitation.Id,
                Slug = project.Slug
            };
        }
    }
}