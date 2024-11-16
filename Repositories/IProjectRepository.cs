using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseForBoardDisplay;
using backend_apis.ApiModels.ResponseForBoardDisplay_v2;
using backend_apis.ApiModels.ResponseForBoardDisplay_v3;
using backend_apis.ApiModels.ResponseForTableDisplay;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Models;

namespace backend_apis.Repositories
{
    public interface IProjectRepository
    {
        Task<ProjectResponseForBoardDisplay?> GetProjectResponseForBoardDisplayAsync(string projectId, string? permission = null, string? userId = null);
        Task<ProjectResponseForTableDisplay?> GetProjectResponseForTableDisplayAsync(string projectId, string? permission = null, string? userId = null);
        Task<Project?> CreateProjectAsync(CreateProjectModel model, string userId);
        Task<ProjectInvitation?> InviteAsync(string pid, string inviterId, InvitationModel model);
        Task<Project?> GetProjectAsync(string projectId);
        Task<ProjectResponseForUpdating?> GetProjectForUpdatingAsync(string projectId);
        Task<Project?> UpdateProjectAsync(string projectId, UpdateProjectModel model);
        Task<ProjectResponseForBoardDisplay_v2?> GetProjectResponseForBoardDisplayAsync_v2(string projectId, string? permission = null, string? userId = null);
        Task<ProjectResponseForBoardDisplay_v3?> GetProjectResponseForBoardDisplayAsync_v3(string projectId, string? permission = null, string? userId = null);
        Task<List<ProjectResponse>?> GetAllProjectsAsync(string userId);
        Task<List<InvitedProjectResponse>?> GetInvitedProjectsAsync(string userId);
        Task<List<InTrashTaskResponse>?> GetRecycleBinAsync(string projectId);
    }
}