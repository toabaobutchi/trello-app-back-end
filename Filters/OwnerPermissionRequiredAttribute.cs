using System.Security.Claims;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace backend_apis.Filters
{
    public class OwnerPermissionRequiredAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public async void OnActionExecuting(ActionExecutingContext context)
        {
            var wAuth = await context.HttpContext.AuthenticateAsync(WorkspaceAuthentication.AuthenticationScheme);
            var pAuth = await context.HttpContext.AuthenticateAsync(ProjectAuthentication.AuthenticationScheme);
            if (!wAuth.Succeeded && !pAuth.Succeeded)
            {
                context.Result = new OkObjectResult(new ApiResponse()
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "No required authentication scheme is satisfiable"
                });
            }
            else
            {
                if (wAuth.Succeeded)
                {
                    var wRole = context.HttpContext.User.FindFirst(WorkspaceClaimType.WorkspaceRole)?.Value;
                    var wId = context.HttpContext.User.FindFirst(WorkspaceClaimType.WorkspaceId)?.Value;
                    if (wRole != ContextResponse.Owner)
                    {
                        context.Result = new OkObjectResult(new ApiResponse()
                        {
                            Status = StatusCodes.Status403Forbidden,
                            Message = $"You don't have enough permissions to access this resource with workspace role: {wRole}"
                        });
                    }
                }
                else if (pAuth.Succeeded)
                {
                    var pRole = context.HttpContext.User.FindFirstValue(ProjectClaimType.ProjectPermission);
                    if (pRole != ContextResponse.Owner && pRole != ContextResponse.Owner)
                    {
                        context.Result = new OkObjectResult(new ApiResponse()
                        {
                            Status = StatusCodes.Status403Forbidden,
                            Message = $"You don't have enough permissions to access this resource with project role: {pRole}"
                        });
                        return;
                    }
                }
            }
        }
    }
}