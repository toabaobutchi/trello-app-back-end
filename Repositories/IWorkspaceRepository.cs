using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Models;

namespace backend_apis.Repositories
{
    public interface IWorkspaceRepository
    {
        Task<List<WorkspaceResponse>> GetOwnWorkspacesAsync(string ownerId);
        Task<Workspace?> CreateWorkspaceAsync(string ownerId, CreateWorkspaceModel model);
        Task<WorkspaceResponseWithRelatedProjects?> GetWorkspaceWithProjectsAsync(int workspaceId, string userId);
        Task<Workspace?> UpdateWorkspacAsync(int workspaceId, string ownerId, WorkspaceUpdateModel model);
        Task<List<Workspace?>?> GetSharedWorkspacesAsync(string userId);
        Task<WorkspaceResponseWithRelatedProjects?> GetSharedWorkspaceWithProjectsAsync(int workspaceId, string userId);
    }
}