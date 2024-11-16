using backend_apis.ApiModels.ContextResModel;
using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Data;
using backend_apis.Filters;
using backend_apis.Repositories;
using backend_apis.Services;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Controllers
{
    [ApiController]
    [Route("/api/projects")]
    [Authorize]
    [ProjectAuthorize]
    public class ProjectController : AppControllerBase
    {
        private readonly ProjectManagerDbContext _db;
        private readonly IProjectRepository _projectRepo;
        private readonly AuthService _authService;
        private readonly IAssignmentRepository _assignmentRepo;
        private readonly EmailService _emailService;

        public ProjectController(ProjectManagerDbContext db, IProjectRepository projectRepo, AuthService authService, IAssignmentRepository assignmentRepo, EmailService emailService)
        {
            _db = db;
            _projectRepo = projectRepo;
            _authService = authService;
            _assignmentRepo = assignmentRepo;
            _emailService = emailService;
        }

        [HttpGet("{pid}/updating")]
        [ProjectRole(ContextResponse.Admin)]
        public async Task<IActionResult> GetProjectForUpdating([FromRoute] string pid)
        {
            var project = await _projectRepo.GetProjectForUpdatingAsync(pid);
            if (project == null)
            {
                return ResponseHelper.NotFound();
            }
            return ResponseHelper.Ok(project, ResponseMessage.S_FETCH);
        }

        [HttpPut("{pid}")]
        [ValidateModel]
        [OwnerPermissionRequired]
        public async Task<IActionResult> UpdateProject([FromRoute] string pid, [FromBody] UpdateProjectModel model)
        {
            var result = await _projectRepo.UpdateProjectAsync(pid, model);
            if (result == null)
            {
                return ResponseHelper.NotFound();
            }
            return ResponseHelper.Ok(UpdateProjectResponse.Create(result), ResponseMessage.S_UPDATE);
        }

        // [HttpGet("projects/{pid}/v/{viewMode}")]
        // [OptionalAuthenticationSchemes]
        // public async Task<IActionResult> GetProject([FromRoute] string pid, [FromRoute] string viewMode)
        // {
        //     var authId = User.FindFirst(UserClaimType.UserId)?.Value;
        //     if (authId == null)
        //     {
        //         return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
        //         {
        //             Status = StatusCodes.Status401Unauthorized,
        //             Message = "Do not have access to this"
        //         });
        //     }
        //     // xác thực workspace và xác thực project
        //     var projectPermission = User.FindFirst(ProjectClaimType.ProjectPermission)?.Value;
        //     dynamic? projectResponse;
        //     if (viewMode == "board")
        //     {
        //         projectResponse = await _projectRepo.GetProjectResponseForBoardDisplayAsync(pid, projectPermission, authId);
        //     }
        //     else
        //     {
        //         projectResponse = await _projectRepo.GetProjectResponseForTableDisplayAsync(pid, projectPermission, authId);
        //     }
        //     if (projectResponse == null)
        //     {
        //         return ResponseHelper.CannotHandle(ResponseMessage.F_FETCH);
        //     }
        //     await _authService.ProjectSignInAsync(projectResponse.Id, projectResponse.Context, TextUtils.CreateAssignmentId(projectResponse.Id, authId));
        //     return Ok(new ApiResponse
        //     {
        //         Status = StatusCodes.Status200OK,
        //         Message = "Successfully get project",
        //         Data = projectResponse
        //     });
        // }

        // [HttpGet("v2/projects/{pid}/v/{viewMode}")]
        // [OptionalAuthenticationSchemes]
        // public async Task<IActionResult> GetProject_v2([FromRoute] string pid, [FromRoute] string viewMode)
        // {
        //     try
        //     {
        //         var authId = User.FindFirst(UserClaimType.UserId)?.Value;
        //         if (authId == null)
        //         {
        //             return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
        //             {
        //                 Status = StatusCodes.Status401Unauthorized,
        //                 Message = "Do not have access to this"
        //             });
        //         }
        //         // xác thực workspace và xác thực project
        //         var projectPermission = User.FindFirst(ProjectClaimType.ProjectPermission)?.Value;
        //         var projectResponse = await _projectRepo.GetProjectResponseForBoardDisplayAsync_v2(pid, projectPermission, authId);
        //         if (projectResponse == null)
        //         {
        //             return Ok(new ApiResponse
        //             {
        //                 Status = StatusCodes.Status404NotFound,
        //                 Message = "Fail to fetch"
        //             });
        //         }
        //         var assignmentId = TextUtils.CreateAssignmentId(projectResponse.Id, authId);
        //         await _authService.ProjectSignInAsync(projectResponse.Id, projectResponse.Context, assignmentId);
        //         projectResponse.AssignmentId = assignmentId;
        //         return Ok(new ApiResponse
        //         {
        //             Status = StatusCodes.Status200OK,
        //             Message = "Successfully get project",
        //             Data = projectResponse
        //         });
        //     }
        //     catch
        //     {
        //         return Ok(new ApiResponse
        //         {
        //             Status = StatusCodes.Status400BadRequest,
        //             Message = "Something went wrong"
        //         });
        //     }
        // }

        [HttpGet("/api/v3/projects/{pid}/v/{viewMode}")]
        [AllowAnonymous]
        [OptionalAuthenticationSchemes]
        public async Task<IActionResult> GetProject_v3([FromRoute] string pid, [FromRoute] string viewMode)
        {
            var authId = UserCookie.UserId;
            var projectPermission = GetProjectCookie().Permission;
            var projectResponse = await _projectRepo.GetProjectResponseForBoardDisplayAsync_v3(pid, projectPermission, authId);

            if (projectResponse == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_FETCH);
            }

            var assignmentId = TextUtils.CreateAssignmentId(projectResponse.Id, authId);
            await _authService.ProjectSignInAsync(projectResponse.Id, projectResponse.Context, assignmentId);
            projectResponse.AssignmentId = assignmentId;

            return ResponseHelper.Ok(projectResponse, ResponseMessage.S_FETCH);
        }
        [HttpPost]
        [WorkspaceOwnerRequired]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectModel model)
        {
            // lấy id từ access token, kiểm tra người gọi request (thông tin này có thể đã bị đánh cắp)
            var authId = GetUserCookie().UserId;

            var result = await _projectRepo.CreateProjectAsync(model, authId);

            if (result == null)
                return ResponseHelper.CannotHandle(ResponseMessage.F_CREATE);

            // ghi cookie cho các phiên request sau
            await _authService.ProjectSignInAsync(result.Id, ContextResponse.Owner, TextUtils.CreateAssignmentId(result.Id, authId));

            return ResponseHelper.Ok(ProjectContextResponse.Create(result, ContextResponse.Owner), ResponseMessage.S_CREATE);
        }

        [HttpPost("{pid}/invite")]
        [ValidateModel]
        public async Task<IActionResult> InviteToProject([FromRoute] string pid, [FromBody] InvitationModel model)
        {
            var authId = GetUserCookie().UserId;
            var invitation = await _projectRepo.InviteAsync(pid, authId, model);

            if (invitation == null)
            {
                return ResponseHelper.CannotHandle();
            }

            var email = Generator.CreateProjectInvitationEmail(new() { InvitedEmail = invitation.InvitedEmail });
            _ = _emailService.SendEmailAsync(email);

            return ResponseHelper.Ok(invitation, ResponseMessage.S_CREATE);
        }

        // [AllowAnonymous]
        [HttpGet("{pid}")]
        public async Task<IActionResult> GetProject([FromRoute] string pid)
        {
            var project = await _projectRepo.GetProjectAsync(pid);
            if (project == null)
            {
                return ResponseHelper.NotFound();
            }
            return ResponseHelper.Ok(project, ResponseMessage.S_FETCH);
        }
        [AllowAnonymous]
        [HttpGet("{pid}/inviter/{uid}")]
        public async Task<IActionResult> GetInviter([FromRoute] string pid, [FromRoute] string uid)
        {
            var assignmentId = TextUtils.CreateAssignmentId(pid, uid);
            var assignment = await _db.Assignments.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == assignmentId);
            if (assignment == null)
            {
                return ResponseHelper.NotFound();
            }
            else
            {
                var assignmentRes = new AssignmentResponse();
                assignmentRes = assignment;

                return ResponseHelper.Ok(assignmentRes, ResponseMessage.S_FETCH);
            }
        }

        [HttpGet("all")]
        [ValidateModel]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> GetAllProjects([FromQuery] string? exceptId = null)
        {
            var authId = UserCookie.UserId;
            var projects = await _projectRepo.GetAllProjectsAsync(authId);
            if (exceptId != null)
            {
                projects = projects.Where(p => p.Id != exceptId).ToList();
            }
            return ResponseHelper.Ok(projects, ResponseMessage.S_FETCH);
        }

        [HttpPost("{pid}/invite/existed-user")]
        [ValidateModel]
        [ProjectRole(ContextResponse.Admin, ContextResponse.Owner)]
        public async Task<IActionResult> InviteExistedUser([FromRoute] string pid, [FromBody] IEnumerable<ExistedMemberInviteModel> model)
        {
            var assignmentId = User.FindFirst(ProjectClaimType.AssignmentId)?.Value;
            var invitations = await _assignmentRepo.InviteExistedMembersAsync(model, pid, assignmentId);
            if (invitations == null)
            {
                return ResponseHelper.CannotHandle();
            }

            return ResponseHelper.Ok(true);
        }

        [HttpGet("invited-projects")]
        public async Task<IActionResult> GetInvitedProjects()
        {
            var authId = UserCookie.UserId;
            var invitation = await _projectRepo.GetInvitedProjectsAsync(authId);
            if (invitation == null)
            {
                return ResponseHelper.NotFound();
            }
            return ResponseHelper.Ok(invitation, ResponseMessage.S_FETCH);
        }

        [HttpGet("{pid}/recycle-bin")]
        public async Task<IActionResult> GetRecycleBin([FromRoute] string pid)
        {
            var result = await _projectRepo.GetRecycleBinAsync(pid);
            if (result == null)
            {
                return ResponseHelper.NotFound();
            }
            return ResponseHelper.Ok(result, ResponseMessage.S_FETCH);
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke()
        {
            await HttpContext.SignOutAsync(ProjectAuthentication.AuthenticationScheme);
            return Ok(new ApiResponse()
            {
                Status = StatusCodes.Status200OK,
                Message = "Successfully revoked project access"
            });
        }
    }
}