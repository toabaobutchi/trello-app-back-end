namespace backend_apis.Repositories
{
    public interface IUserRepository
    {
        Task<Models.User?> FindAsync(params object?[]? id);
    }
}