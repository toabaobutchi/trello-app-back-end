using backend_apis.ApiModels.ResponseModels;
using backend_apis.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace backend_apis.Filters
{
    public class ProjectRoleAttribute : Attribute, IActionFilter
    {
        private readonly string[] _roles = [];
        public ProjectRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }
        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            if (user.Identity?.AuthenticationType != ProjectAuthentication.AuthenticationScheme || !user.Identity.IsAuthenticated)
            {
                context.Result = new OkObjectResult(new ApiResponse()
                {
                    Status = StatusCodes.Status403Forbidden,
                    Message = "No session found"
                });
            }
            else
            {
                if (!_roles.Contains(user.FindFirst(ProjectClaimType.ProjectPermission)?.Value))
                {
                    context.Result = new OkObjectResult(new ApiResponse()
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "You don't have enough permissions to access this resource"
                    });
                }
            }
        }
    }
}