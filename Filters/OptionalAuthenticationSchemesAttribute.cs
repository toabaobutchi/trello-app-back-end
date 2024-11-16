using backend_apis.ApiModels.ResponseModels;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace backend_apis.Filters
{
    public class OptionalAuthenticationSchemesAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) { }

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
                return;
            }
        }
    }
}