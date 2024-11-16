using backend_apis.Utils;
using Microsoft.AspNetCore.Authorization;

namespace backend_apis.Filters
{
    public class ProjectAuthorize : AuthorizeAttribute
    {
        public ProjectAuthorize()
        {
            AuthenticationSchemes = ProjectAuthentication.AuthenticationScheme;
            Policy = ProjectAuthentication.RequiredPolicy;
        }
    }
}