using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend_apis.Utils;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace backend_apis.Services
{
    public class AuthService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly string? _googleClientId;
        private readonly string? _accessTokenSecretKey;
        private readonly string? _refreshTokenSecretKey;

        public AuthService(IHttpContextAccessor accessor, IConfiguration configuration)
        {
            _accessor = accessor;
            _googleClientId = configuration[AppSettings.GoogleClientId];
            _accessTokenSecretKey = configuration[AppSettings.AccessTokenSecretKey];
            _refreshTokenSecretKey = configuration[AppSettings.RefreshTokenSecretKey];
        }
        public (string?, string?) GetTokens(HttpRequest request)
        {
            var accessToken = request.Headers[AuthConstant.Authorization].ToString().Split(' ')[1];
            var refreshToken = request.Cookies[AuthConstant.RefreshToken];
            return (accessToken, refreshToken);
        }
        public async Task WorkspaceSignInAsync(int workspaceId, string role)
        {
            var claims = new List<Claim>() {
                        new Claim(WorkspaceClaimType.WorkspaceRole, role),
                        new Claim(WorkspaceClaimType.WorkspaceId, workspaceId.ToString()),
                    };
            var identity = new ClaimsIdentity(claims, WorkspaceAuthentication.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await _accessor.HttpContext.SignInAsync(WorkspaceAuthentication.AuthenticationScheme, principal);
        }
        public async Task ProjectSignInAsync(string projectId, string permission, string assignmentId)
        {
            var claims = new List<Claim>() {
                        new Claim(ProjectClaimType.ProjectId, projectId),
                        new Claim(ProjectClaimType.AssignmentId, assignmentId),
                        new Claim(ProjectClaimType.ProjectPermission, permission),
                    };
            var identity = new ClaimsIdentity(claims, ProjectAuthentication.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await _accessor.HttpContext.SignInAsync(ProjectAuthentication.AuthenticationScheme, principal);
        }
        public async Task<GoogleJsonWebSignature.Payload?> GetGooglePayLoad(string credentials)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = [_googleClientId],
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(credentials, settings);
                return payload;
            }
            catch
            {
                return null;
            }
        }
        public string GenerateAccessToken(Models.User user)
        {
            var userClaims = JwtUtils.GetUserClaims(user);
            return JwtUtils.Sign(userClaims, _accessTokenSecretKey, DateTime.UtcNow.AddHours(1));
        }
        public string GenerateRefreshToken()
        {
            return JwtUtils.Sign([], _refreshTokenSecretKey, DateTime.UtcNow.AddDays(15));
        }
        public void WriteRefreshTokenToCookie()
        {
            var refreshToken = GenerateRefreshToken();
            var cookieOptions = CookieUtils.Create(DateTime.Now.AddDays(15));
            _accessor.HttpContext?.Response.Cookies.Append(AuthConstant.RefreshToken, refreshToken, cookieOptions);
        }
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken, string refreshToken)
        {
            try
            {
                var isValidRefreshToken = JwtUtils.Verify(refreshToken, _refreshTokenSecretKey, out _);
                if (!isValidRefreshToken) return null;
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessTokenSecretKey)),
                    ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}