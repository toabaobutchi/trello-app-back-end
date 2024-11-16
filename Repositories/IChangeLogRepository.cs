using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;

namespace backend_apis.Repositories
{
    public interface IChangeLogRepository
    {
        Task<Models.ChangeLog?> WriteChangeLogAsync(Models.ChangeLog changeLog);
        Task<List<ChangeLogResponse>?> GetChangeLogsAsync(string projectId, int offset = 0, int count = 20, long? date = null, string? uid = null, string? task = null);
        Task<List<ChangeLogResponse>?> GetChangeLogsAsync(string projectId, ChangeLogPagination? logPagination);
    }
}