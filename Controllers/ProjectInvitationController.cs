using backend_apis.ApiModels.ResponseModels;
using backend_apis.Repositories;
using backend_apis.Services;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_apis.Controllers
{
    [ApiController]
    [Route("api/invitations")]
    [Authorize]
    public class ProjectInvitationController : ControllerBase
    {
        private readonly IInvitationRepository _invitationRepo;
        private readonly AuthService _authService;

        public ProjectInvitationController(IInvitationRepository invitationRepo, AuthService authService)
        {
            _invitationRepo = invitationRepo;
            _authService = authService; // injecting auth service for user authentication
        }
        [HttpPost("{iid}/{handle}")]
        public async Task<IActionResult> HandleInvitation([FromRoute] string iid, [FromRoute] string handle)
        {
            var invitation = await _invitationRepo.HandleProjectInvitationAsync(iid, handle);
            if (invitation == null)
            {
                return ResponseHelper.CannotHandle();
            }
            if (invitation.IsAccepted)
            {
                // ghi phiên đăng nhập để cho front end chuyển hướng đến
                await _authService.ProjectSignInAsync(invitation.ProjectId, invitation.Context, invitation.AssignmentId);
            }
            return ResponseHelper.Ok(invitation);
        }
    }
}