using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseForBoardDisplay;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Filters;
using backend_apis.Repositories;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_apis.Controllers
{
    [ApiController]
    [Route("api/subtasks")]
    [Authorize]
    [ProjectAuthorize]
    public class SubtaskController : ProjectControllerBase
    {
        private readonly ISubtaskRepository _subtaskRepo;

        public SubtaskController(ISubtaskRepository subtaskRepo)
        {
            _subtaskRepo = subtaskRepo;
        }

        [HttpPost("{sid}/join")]
        public async Task<IActionResult> JoinSubtask([FromRoute] int sid)
        {
            var assignmentId = ProjectCookie.AssignmentId;
            var joinSubtaskResponse = await _subtaskRepo.JoinSubtaskAsync(sid, assignmentId);
            if (joinSubtaskResponse == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.CAN_NOT_JOIN);
            }
            return ResponseHelper.Ok(joinSubtaskResponse);
        }

        [HttpPost("{sid}/assign")]
        [ValidateModel]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> AssignToSubtask([FromRoute] int sid, [FromBody] AssignSubtaskModel model)
        {
            var assignerId = ProjectCookie.AssignmentId;
            var assignSubtaskResponse = await _subtaskRepo.AssignSubtaskAsync(sid, assignerId, model);
            if (assignSubtaskResponse == null)
            {
                return ResponseHelper.CannotHandle();
            }
            return ResponseHelper.Ok(assignSubtaskResponse);
        }

        [HttpPost("{sid}/unassign")]
        [ValidateModel]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> UnassignSubtask([FromRoute] int sid, [FromBody] UnassignSubtaskModel model)
        {
            var unassignResult = await _subtaskRepo.RemoveAssignmentFromSubtaskAsync(sid, model);
            if (unassignResult == null)
            {
                return ResponseHelper.CannotHandle();
            }
            return ResponseHelper.Ok(unassignResult, ResponseMessage.S_DELETE);
        }

        [HttpPut("{sid}/change-name")]
        [ValidateModel]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> ChangeSubtaskName([FromRoute] int sid, [FromBody] ChangeSubtaskNameModel model)
        {
            var permission = ProjectCookie.Permission;
            var updatedName = await _subtaskRepo.ChangeSubtaskNameAsync(model.Name, sid, permission);
            if (updatedName == null)
            {
                return ResponseHelper.CannotHandle();
            }
            return ResponseHelper.Ok(updatedName, ResponseMessage.S_UPDATE);
        }

        [HttpDelete("{sid}")]
        [ValidateModel]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> DeleteSubtask([FromRoute] int sid)
        {
            var permission = ProjectCookie.Permission;
            var deletedSubtask = await _subtaskRepo.DeleleSubtaskAsync(sid, permission);
            if (deletedSubtask == null)
            {
                return ResponseHelper.CannotHandle();
            }

            return ResponseHelper.Ok(SubtaskResponse.Create(deletedSubtask), ResponseMessage.S_DELETE);
        }
        [HttpPost]
        [ValidateModel]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> CreateSubtask([FromBody] CreateSubtaskModel model)
        {
            var subtasks = await _subtaskRepo.CreateSubtasksAsync(model);
            if (subtasks == null)
            {
                return ResponseHelper.CannotHandle();
            }
            var subtasksResponse = subtasks.Select(s => SubtaskForBoard.Create(s));
            return ResponseHelper.Ok(subtasksResponse, ResponseMessage.S_CREATE);
        }

        [HttpPut("{sid}/change-status")]
        [ValidateModel]
        public async Task<IActionResult> ChangeSubtaskStatus([FromRoute] int sid, [FromBody] ChangeSubtaskStatusModel model)
        {
            var result = await _subtaskRepo.ChangeSubtaskStatusAsync(model, sid, ProjectCookie);

            if (result == null)
            {
                return ResponseHelper.CannotHandle();
            }
            return ResponseHelper.Ok(result, ResponseMessage.S_UPDATE);
        }
    }
}