using backend_apis.ApiModels.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace backend_apis.Utils
{
    public static class ResponseHelper
    {
        private static OkObjectResult CreateOkResponse(int statusCode, object? data = null, string? message = null)
        {
            return new OkObjectResult(new ApiResponse()
            {
                Status = statusCode,
                Message = message,
                Data = data
            });
        }
        public static OkObjectResult Ok(object? data = null, string? message = null)
        {
            return CreateOkResponse(StatusCodes.Status200OK, data, message);
        }
        public static OkObjectResult BadRequest(object? data = null, string? message = null)
        {
            return CreateOkResponse(StatusCodes.Status400BadRequest, data, message);
        }
        public static OkObjectResult NotFound(string? message = null, object? data = null)
        {
            return CreateOkResponse(StatusCodes.Status404NotFound, data, message ?? ResponseMessage.NOT_FOUND);
        }
        public static OkObjectResult CannotHandle(string? message = null, object? data = null)
        {
            return CreateOkResponse(StatusCodes.Status400BadRequest, data, message ?? ResponseMessage.CAN_NOT_HANDLE);
        }
        public static OkObjectResult InternalServerError(object? data = null)
        {
            int status = StatusCodes.Status500InternalServerError;
            string message = ResponseMessage.INTERNAL_SERVER_ERROR;
            return CreateOkResponse(status, data, message);
        }
        public static UnauthorizedObjectResult Unauthorized(object? data = null, string? message = null)
        {
            return new UnauthorizedObjectResult(new ApiResponse()
            {
                Status = StatusCodes.Status401Unauthorized,
                Message = message,
                Data = data
            });
        }
    }
}