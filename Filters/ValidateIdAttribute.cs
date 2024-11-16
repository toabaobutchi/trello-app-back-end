using backend_apis.ApiModels.ResponseModels;
using backend_apis.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace backend_apis.Filters
{
    public class ValidateIdAttribute : Attribute, IActionFilter
    {
        private Func<ActionExecutingContext, string> _idGetter;
        public ValidateIdAttribute(Func<ActionExecutingContext, string> idGetter)
        {
            _idGetter = idGetter;
        }
        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var projectId = context.HttpContext.User.FindFirst(ProjectClaimType.ProjectId)?.Value;
            var id = _idGetter(context);
            if (string.IsNullOrEmpty(id) || id != projectId)
            {
                context.Result = new ObjectResult(new ApiResponse
                {
                    Status = StatusCodes.Status403Forbidden,
                    Message = "Access denied"
                });
            }
        }
    }
}