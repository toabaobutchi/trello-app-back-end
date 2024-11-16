using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Filters;
using backend_apis.Repositories;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_apis.Controllers
{
    [ApiController]
    [Route("/api/assignments")]
    [Authorize]
    [ProjectAuthorize]
    public class AssignmentController : AppControllerBase
    {
        private readonly IAssignmentRepository _assignmentRepo;

        public AssignmentController(IAssignmentRepository assignmentRepo)
        {
            _assignmentRepo = assignmentRepo;
        }
        [HttpGet("{aid}/profile")]
        public async Task<IActionResult> GetAssignmentProfile([FromRoute] string aid)
        {
            var assignment = await _assignmentRepo.GetAssignmentProfileAsync(aid);
            if (assignment == null)
            {
                return ResponseHelper.NotFound();
            }
            return ResponseHelper.Ok(assignment, ResponseMessage.S_FETCH);
        }
        [HttpGet("in-project/{pid}")]
        [OptionalAuthenticationSchemes]
        public async Task<IActionResult> GetAssignmentsByProject([FromRoute] string pid, [FromQuery] bool? exceptMe = null)
        {
            var authId = UserCookie.UserId;
            var assignments = await _assignmentRepo.GetAssignmentsByProject(pid);
            if (assignments == null)
            {
                return ResponseHelper.NotFound();
            }
            if (exceptMe != null && exceptMe == true)
            {
                assignments = assignments.Where(a => a.UserId != authId).ToList();
            }
            return ResponseHelper.Ok(assignments, ResponseMessage.S_FETCH);
        }
        [HttpGet("in-other-project/{opid}")]
        public async Task<IActionResult> GetAssignmentsFromAnotherProject([FromRoute] string opid)
        {
            var currentProjectId = ProjectCookie.ProjectId;
            var userId = UserCookie.UserId;
            var result = await _assignmentRepo.GetAssignmentsFromAnotherProject(currentProjectId, opid, userId);
            if (result == null)
            {
                return ResponseHelper.NotFound();
            }
            return ResponseHelper.Ok(result, ResponseMessage.S_FETCH);
        }
        [HttpPost("assign-to-task/{tid}")]
        [ValidateModel]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> AssignToTask([FromRoute] string tid, [FromBody] AssignByTaskModel model)
        {
            var uid = UserCookie.UserId;
            var assignmentId = ProjectCookie.AssignmentId;
            var result = await _assignmentRepo.AssignByTaskAsync(tid, model, uid, assignmentId);

            if (result == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_CREATE);
            }
            return ResponseHelper.Ok(result, ResponseMessage.S_CREATE);
        }

        [HttpPost("unassign-from-task/{tid}")]
        [ValidateModel]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> UnassignFromTask([FromRoute] string tid, [FromBody] DeleteTaskAssignmentModel model)
        {
            var result = await _assignmentRepo.DeleteTaskAssignment(tid, model);
            if (result == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_DELETE);
            }
            return ResponseHelper.Ok(result, ResponseMessage.S_DELETE);
        }

        [HttpDelete("remove/{aid}")]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> DeleteAssignment([FromRoute] string aid)
        {
            var assignmentId = ProjectCookie.AssignmentId;
            var result = await _assignmentRepo.DeleteAssignmentAsync(assignmentId, aid);

            if (result == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_DELETE);
            }
            return ResponseHelper.Ok(result, ResponseMessage.S_DELETE);
        }

        [HttpPut("change-permission/{aid}")]
        [ValidateModel]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> ChangePermission([FromRoute] string aid, [FromBody] ChangePermissionModel model)
        {
            var assignmentId = ProjectCookie.AssignmentId;
            var projectId = ProjectCookie.ProjectId;
            var result = await _assignmentRepo.ChangePermissionAsync(assignmentId, projectId, aid, model);

            if (result == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_UPDATE);
            }
            return ResponseHelper.Ok(result, ResponseMessage.S_UPDATE);
        }
    }
}