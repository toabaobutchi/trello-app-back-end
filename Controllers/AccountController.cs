using backend_apis.ApiModels.RequestModels;
using backend_apis.ApiModels.ResponseModels;
using backend_apis.Repositories;
using backend_apis.Services;
using backend_apis.Utils;
using Microsoft.AspNetCore.Mvc;

#nullable disable

namespace backend_apis.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AccountController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IAuthRepository _authRepo;
        private readonly IUserRepository _userRepo;

        public AccountController(AuthService authService, IAuthRepository authRepo, IUserRepository userRepo)
        {
            _authService = authService;
            _authRepo = authRepo;
            _userRepo = userRepo;
        }
        [HttpPost("login")]
        public async Task<IActionResult> SignIn([FromBody] SignInModel model)
        {
            string credentials = model.Credentials;
            var payload = await _authService.GetGooglePayLoad(credentials);

            var grantResult = await _authRepo.CheckAndGrantAccessTokenAsync(payload);

            _authService.WriteRefreshTokenToCookie();

            return ResponseHelper.Ok(new AuthResponse(grantResult.AccessToken, grantResult.User), ResponseMessage.S_LOGIN);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var (accessToken, refreshToken) = _authService.GetTokens(Request);
            var principal = _authService.GetPrincipalFromExpiredToken(accessToken, refreshToken);

            if (principal == null)
            {
                return ResponseHelper.Unauthorized(refreshToken, ResponseMessage.INVALID_TOKEN);
            }

            var uid = principal.FindFirst(UserClaimType.UserId)?.Value;
            var user = await _userRepo.FindAsync(uid);
            if (user == null)
            {
                return ResponseHelper.Unauthorized(refreshToken, ResponseMessage.USER_NOT_FOUND);
            }

            var newAccessToken = _authService.GenerateAccessToken(user);
            _authService.WriteRefreshTokenToCookie();

            return ResponseHelper.Ok(new AuthResponse(newAccessToken, user), ResponseMessage.S_RENEW_TOKEN);
        }
    }
}