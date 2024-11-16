using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Utils;

namespace backend_apis.Repositories
{
    public interface ICommentRepository
    {
        Task<List<ProjectCommentResponse>?> GetProjectCommentsAsync(string projectId);
        Task<ProjectCommentResponse?> CreateProjectCommentAsync(ProjectCookie projectCookie, CreateProjectCommentModel model);
    }
}