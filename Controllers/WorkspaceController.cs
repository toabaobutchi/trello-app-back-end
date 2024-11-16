using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Filters;
using backend_apis.Repositories;
using backend_apis.Services;
using backend_apis.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#nullable disable

namespace backend_apis.Controllers
{
    [ApiController]
    [Route("api/workspaces")]
    [Authorize]
    public class WorkspaceController : AppControllerBase
    {
        private readonly IWorkspaceRepository _workspaceRepo;
        private readonly AuthService _authService;

        public WorkspaceController(IWorkspaceRepository workspaceRepo, AuthService authService)
        {
            _workspaceRepo = workspaceRepo;
            _authService = authService;
        }
        [HttpGet]
        public async Task<IActionResult> GetWorkspaces()
        {
            var workspaces = await _workspaceRepo.GetOwnWorkspacesAsync(UserCookie.UserId);
            return ResponseHelper.Ok(workspaces, ResponseMessage.S_FETCH);
        }
        
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] CreateWorkspaceModel model)
        {
            var workspace = await _workspaceRepo.CreateWorkspaceAsync(UserCookie.UserId, model);
            if (workspace == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_CREATE);
            }
            var data = WorkspaceResponse.Create(workspace, ContextResponse.Owner);
            return ResponseHelper.Ok(data, ResponseMessage.S_CREATE);
        }

        [HttpGet("{wid}/projects")]
        public async Task<IActionResult> GetProjectsByWorkspace([FromRoute] int wid)
        {
            var workspace = await _workspaceRepo.GetWorkspaceWithProjectsAsync(wid, UserCookie.UserId);
            if (workspace == null)
            {
                return ResponseHelper.NotFound();
            }
            else
            {
                // ghi lại phiên làm việc với workspace
                await _authService.WorkspaceSignInAsync(workspace.Id, ContextResponse.Owner);

                return ResponseHelper.Ok(workspace, ResponseMessage.S_FETCH);
            }
        }

        [HttpPut("{wid}")]
        [ValidateModel]
        [WorkspaceAuthorize]
        public async Task<IActionResult> UpdateWorkspace([FromRoute] int wid, [FromBody] WorkspaceUpdateModel model)
        {
            var workspace = await _workspaceRepo.UpdateWorkspacAsync(wid, UserCookie.UserId, model);
            if (workspace == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_UPDATE);
            }
            return ResponseHelper.Ok(WorkspaceResponse.Create(workspace, ContextResponse.Owner), ResponseMessage.S_UPDATE);
        }

        [HttpGet("/api/shared-workspaces")]
        public async Task<IActionResult> GetSharedWorkspaces()
        {
            var sharedWorkspaces = await _workspaceRepo.GetSharedWorkspacesAsync(UserCookie.UserId);
            var sharedWorkspaceResponse = sharedWorkspaces.Select(sw => WorkspaceResponse.Create(sw, ContextResponse.Member));
            return ResponseHelper.Ok(sharedWorkspaceResponse, ResponseMessage.S_FETCH);
        }

        [HttpGet("/api/shared-workspaces/{wid}/projects")]
        public async Task<IActionResult> GetProjectsBySharedWorkspace([FromRoute] int wid)
        {
            var authId = User.FindFirst(UserClaimType.UserId)?.Value;
            var result = await _workspaceRepo.GetSharedWorkspaceWithProjectsAsync(wid, authId);
            if (result == null)
            {
                return ResponseHelper.CannotHandle(ResponseMessage.F_FETCH);
            }
            await _authService.WorkspaceSignInAsync(result.Id, ContextResponse.Member);

            return ResponseHelper.Ok(result, ResponseMessage.S_FETCH);
        }
    }
}