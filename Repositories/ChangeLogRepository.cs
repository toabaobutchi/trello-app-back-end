using backend_apis.ApiModels;
using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Data;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Repositories
{
    public class ChangeLogRepository : IChangeLogRepository
    {
        private readonly ProjectManagerDbContext _db;
        public ChangeLogRepository(ProjectManagerDbContext db)
        {
            _db = db;
        }

        public async Task<List<ChangeLogResponse>?> GetChangeLogsAsync(string projectId, int offset, int count, long? date = null, string? uid = null, string? task = null)
        {
            try
            {
                offset = offset <= 0 ? 1 : offset;
                count = count <= 0 ? 20 : count;
                var changeLogs = await _db.ChangeLogs
                    .AsNoTracking()
                    .AsAsyncEnumerable()
                    .Where(c =>
                        c.ProjectId == projectId
                        && (date == null ||
                            (DateTimeOffset.FromUnixTimeMilliseconds(date.Value).DateTime.Date == DateTimeOffset.FromUnixTimeMilliseconds(c.CreatedAt).DateTime.Date))
                        && (uid == null || (c.AssignmentId == uid))
                        && (task == null || (LogDetail.Parse(c.Log).EntityId.ToString() == task))
                    )
                    .OrderByDescending(c => c.CreatedAt)
                    .Skip((offset - 1) * count)
                    .Take(count)
                    .ToListAsync();
                if (changeLogs == null)
                {
                    return [];
                }
                return changeLogs.Select(ChangeLogResponse.Create).ToList();
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<ChangeLogResponse>?> GetChangeLogsAsync(string projectId, ChangeLogPagination? logPagination)
        {
            return await GetChangeLogsAsync(projectId, logPagination?.Offset ?? 0, logPagination?.Count ?? 20, logPagination?.Date, logPagination?.Uid, logPagination?.Task);
        }

        public async Task<Models.ChangeLog?> WriteChangeLogAsync(Models.ChangeLog changeLog)
        {
            try
            {
                await _db.ChangeLogs.AddAsync(changeLog);
                await _db.SaveChangesAsync();
                return changeLog;
            }
            catch
            {
                return null;
            }
        }
    }
}