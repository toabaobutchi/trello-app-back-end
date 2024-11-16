using backend_apis.ApiModels.RequestModels;
using backend_apis.Repositories;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_apis.Controllers
{
    [ApiController]
    [Route("api/changelogs")]
    public class ChangeLogController : ControllerBase
    {
        private readonly IChangeLogRepository _changeLogRepo;

        public ChangeLogController(IChangeLogRepository changeLogRepo)
        {
            _changeLogRepo = changeLogRepo;
        }
        [HttpGet("in-project/{pid}")]
        [Authorize(AuthenticationSchemes = ProjectAuthentication.AuthenticationScheme, Policy = ProjectAuthentication.RequiredPolicy)]
        public async Task<IActionResult> GetChangeLogs([FromRoute] string pid, [FromQuery] ChangeLogPagination logPagination)
        {
            var changeLogs = await _changeLogRepo.GetChangeLogsAsync(pid, logPagination);
            if (changeLogs == null)
            {
                return ResponseHelper.CannotHandle();
            }

            return ResponseHelper.Ok(changeLogs, ResponseMessage.S_FETCH);
        }
    }
}