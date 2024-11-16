using System.Security.Claims;

namespace backend_apis.Utils
{
    public class UserCookie
    {
        public string DisplayName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Avatar { get; set; } = null!;
        public string UserId { get; set; } = null!;

        public UserCookie(ClaimsPrincipal principal)
        {
            DisplayName = principal.FindFirstValue(UserClaimType.DisplayName) ?? "";
            Email = principal.FindFirstValue(UserClaimType.Email) ?? "";
            Avatar = principal.FindFirstValue(UserClaimType.Avatar) ?? "";
            UserId = principal.FindFirstValue(UserClaimType.UserId) ?? "";
        }
        public bool IsValid
        {
            get {
                return !string.IsNullOrEmpty(DisplayName) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Avatar) && !string.IsNullOrEmpty(UserId);
            }
        }
    }
}