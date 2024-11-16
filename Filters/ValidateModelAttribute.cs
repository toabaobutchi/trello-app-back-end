using backend_apis.ApiModels.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace backend_apis.Filters
{
    public class ValidateModelAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var res = new ApiResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Some fields are required",
                    Data = context.ModelState.Values
                };
                context.Result = new OkObjectResult(res);
            }
        }
    }
}