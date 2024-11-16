using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Data;
using backend_apis.Models;
using backend_apis.Utils;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ProjectManagerDbContext _db;

        public CommentRepository(ProjectManagerDbContext db)
        {
            _db = db;
        }

        public async Task<ProjectCommentResponse?> CreateProjectCommentAsync(ProjectCookie cookie, CreateProjectCommentModel model)
        {
            try
            {
                var (projectId, assignmentId, _) = cookie.Destruct();
                var projectComment = new ProjectComment()
                {
                    ProjectId = projectId,
                    AssignmentId = assignmentId,
                    Content = model.Content,
                    CommentAt = DateTimeUtils.GetSeconds(),
                    Id = Guid.NewGuid().ToString(),
                };
                var result = await _db.ProjectComments.AddAsync(projectComment);
                await _db.SaveChangesAsync();
                return ProjectCommentResponse.Create(result.Entity);
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<ProjectCommentResponse>?> GetProjectCommentsAsync(string projectId)
        {
            try
            {
                var comments = await _db.ProjectComments
                    .AsNoTracking()
                    .Where(pc => pc.ProjectId == projectId)
                    .Select(pc => ProjectCommentResponse.Create(pc))
                    .ToListAsync();
                return comments;
            }
            catch
            {
                return null;
            }
        }
    }
}