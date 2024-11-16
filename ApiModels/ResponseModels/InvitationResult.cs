using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record InvitationResult
    {
        public string ProjectId { get; set; } = null!;
        public string? ProjectName { get; set; }
        public string? ProjectSlug { get; set; }
        public string InvitationId { get; set; } = null!;
        public string? AssignmentId { get; set; }
        public string? Context { get; set; }
        public bool IsAccepted { get; set; }

        public static InvitationResult Create(Models.ProjectInvitation invitation, Project? project, bool isAccepted, string? assignmentId = null)
        {
            return new InvitationResult()
            {
                ProjectId = invitation.ProjectId,
                InvitationId = invitation.Id,
                AssignmentId = assignmentId,
                Context = invitation.Permission.ToString().ToLower(),
                IsAccepted = isAccepted,
                ProjectName = project?.Name,
                ProjectSlug = project?.Slug,
            };
        }
    }
}