using backend_apis.Utils;
using Microsoft.AspNetCore.Diagnostics;

namespace backend_apis.Handlers
{
    public class ExceptionHandler : IExceptionHandler
    {
        public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = StatusCodes.Status200OK;
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.WriteAsJsonAsync(ResponseHelper.InternalServerError(null));
            return ValueTask.FromResult(true);
        }
    }
}