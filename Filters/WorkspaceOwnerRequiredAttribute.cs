using System.Security.Claims;
using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Data;
using backend_apis.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace backend_apis.Filters
{
    public class WorkspaceOwnerRequiredAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            // lấy thông tin model
            var model = context.ActionArguments.Values.FirstOrDefault() as CreateProjectModel;
            // lấy thông tin workspace
            var workspaceId = user.FindFirstValue(WorkspaceClaimType.WorkspaceId);

            // không có xác thực từ workspace hoặc không xác thực được (vì từ workspace khác)
            if (workspaceId == null || workspaceId != model?.WorkspaceId.ToString())
            {
                var db = context.HttpContext.RequestServices.GetRequiredService<ProjectManagerDbContext>();
                var workspace = db.Workspaces.Find(model?.WorkspaceId);

                // không tìm thấy workspace yêu cầu
                if (workspace == null)
                {
                    context.Result = new OkObjectResult(new ApiResponse()
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = "Workspace not found or not authenticated"
                    });
                    return;
                }
                var userId = user.FindFirstValue(UserClaimType.UserId);
                if (workspace?.OwnerId != userId)
                {
                    // không phải chủ workspace
                    context.Result = new OkObjectResult(new ApiResponse()
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "You don't have enough permissions to access this resource"
                    });
                    return;
                }
            }
            else
            {
                // trường hợp đang có xác thực workspace hợp lệ
                var workspaceRole = user.FindFirstValue(WorkspaceClaimType.WorkspaceRole);
                if (workspaceRole != ContextResponse.Owner)
                {
                    context.Result = new OkObjectResult(new ApiResponse()
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "You don't have enough permissions to access this resource"
                    });
                    return;
                }
            }
        }
    }
}