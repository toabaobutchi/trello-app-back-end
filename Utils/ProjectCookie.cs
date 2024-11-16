using System.Security.Claims;

namespace backend_apis.Utils
{
    public sealed record ProjectCookie : IDestructibleObject<(string, string, string)>
    {
        public string ProjectId { get; set; }
        public string AssignmentId { get; set; }
        public string Permission { get; set; }
        public ProjectCookie(ClaimsPrincipal principal)
        {
            ProjectId = principal.FindFirstValue(ProjectClaimType.ProjectId) ?? "";
            AssignmentId = principal.FindFirstValue(ProjectClaimType.AssignmentId) ?? "";
            Permission = principal.FindFirstValue(ProjectClaimType.ProjectPermission) ?? "";
        }
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(ProjectId) && !string.IsNullOrEmpty(AssignmentId) && !string.IsNullOrEmpty(Permission);
            }
        }

        public (string, string, string) Destruct()
        {
            return (ProjectId, AssignmentId, Permission);
        }
    }
}
