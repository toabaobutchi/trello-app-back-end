using backend_apis.Data;
using backend_apis.Models;
using backend_apis.Services;
using Google.Apis.Auth;

namespace backend_apis.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ProjectManagerDbContext _db;
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public AuthRepository(ProjectManagerDbContext db, UserService userService, AuthService authService)
        {
            _db = db;
            _userService = userService;
            _authService = authService;
        }

        public async Task<(string AccessToken, User User)> CheckAndGrantAccessTokenAsync(GoogleJsonWebSignature.Payload payload)
        {
            var user = _userService.CreateUserFromPayLoad(payload);

            var existedUser = _db.Users.Find(user.Id);

            string accessToken = "";
            if (existedUser != null) // nếu đã tồn tại thì đăng nhập
            {
                accessToken = _authService.GenerateAccessToken(existedUser);
            }
            else
            {
                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                accessToken = _authService.GenerateAccessToken(user);
            }
            return (accessToken, user);
        }
    }
}