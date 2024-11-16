using backend_apis.Data;
using backend_apis.Models;

namespace backend_apis.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ProjectManagerDbContext _db;

        public UserRepository(ProjectManagerDbContext db)
        {
            _db = db;
        }
        public async Task<User?> FindAsync(params object?[]? id)
        {
            try
            {
                return await _db.Users.FindAsync(id);
            }
            catch
            {
                return null;
            }
        }
    }
}