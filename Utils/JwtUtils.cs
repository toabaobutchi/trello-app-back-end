using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend_apis.Models;
using Microsoft.IdentityModel.Tokens;

namespace backend_apis.Utils
{
    public class JwtUtils
    {
        public static string Sign(IEnumerable<Claim> data, string secretKey, DateTime? expireInMinutes)
        {
            if (string.IsNullOrEmpty(secretKey)) return string.Empty;
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(data),
                Expires = expireInMinutes,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        public static Claim[] GetUserClaims(User user)
        {
            return [
                new Claim(UserClaimType.DisplayName, user.DisplayName),
                new Claim(UserClaimType.Email, user.Email),
                new Claim(UserClaimType.Avatar, user.Avatar),
                new Claim(UserClaimType.UserId, user.Id)
            ];
        }
        public static bool Verify(string token, string secretKey, out SecurityToken? validatedToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            try
            {
                var tokenValidateParams = new TokenValidationParameters
                {
                    // tự cấp token
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    // ký vào token
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes)
                };
                tokenHandler.ValidateToken(token, tokenValidateParams, out validatedToken);
                return true;
            }
            catch
            {
                validatedToken = null;
                return false;
            }
        }

        // public static ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        // {
        //     var tokenValidationParameters = new TokenValidationParameters
        //     {
        //         ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
        //         ValidateIssuer = false,
        //         ValidateIssuerSigningKey = true,
        //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes()),
        //         ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
        //     };
        //     var tokenHandler = new JwtSecurityTokenHandler();
        //     SecurityToken securityToken;
        //     var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        //     var jwtSecurityToken = securityToken as JwtSecurityToken;
        //     if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //         throw new SecurityTokenException("Invalid token");
        //     return principal;
        // }
        // private readonly IConfiguration configuration;

        // public JwtUtils(IConfiguration configuration)
        // {
        //     this.configuration = configuration;
        // }
        // public string GenerateToken(User user)
        // {
        //     var token = new JwtSecurityTokenHandler();
        //     var secretKeyBytes = Encoding.UTF8.GetBytes(configuration["AppSettings:SecretKey"] ?? "");
        //     var tokenDescriptor = new SecurityTokenDescriptor
        //     {
        //         Subject = new ClaimsIdentity([
        //             new Claim("DisplayName", user.DisplayName ?? ""),
        //             new Claim(ClaimTypes.Email, user.Email)
        //         ]),
        //         Expires = DateTime.UtcNow.AddMinutes(1),
        //         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
        //     };
        //     return token.WriteToken(token.CreateToken(tokenDescriptor));
        // }
        //     public string RenewToken(string oldToken, User user, int extendedTime = 15)
        //     {
        //         var tokenHandler = new JwtSecurityTokenHandler();
        //         var secretKeyBytes = Encoding.UTF8.GetBytes(configuration["AppSettings:SecretKey"] ?? "");
        //         var tokenValidateParams = new TokenValidationParameters
        //         {
        //             // tự cấp token
        //             ValidateIssuer = false,
        //             ValidateAudience = false,
        //             ValidateLifetime = false,
        //             // ký vào token
        //             ValidateIssuerSigningKey = true,
        //             IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
        //             ClockSkew = TimeSpan.Zero,
        //         };
        //         try
        //         {
        //             var tokenValidationResult = tokenHandler.ValidateToken(oldToken, tokenValidateParams, out var validatedToken);
        //             var jwtSecurityToken = (JwtSecurityToken)validatedToken;
        //             var expireTime = long.Parse(jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "Expires").Value);
        //             var expireDate = new DateTime(1970, 1, 1).AddSeconds(expireTime).ToUniversalTime();
        //             if (expireDate - DateTime.Now < TimeSpan.FromMinutes(5))
        //             {
        //                 var tokenDescriptor = new SecurityTokenDescriptor
        //                 {
        //                     Subject = new ClaimsIdentity([
        //                         new Claim("DisplayName", user.DisplayName?? ""),
        //                         new Claim(ClaimTypes.Email, user.Email)
        //                     ]),
        //                     Expires = DateTime.UtcNow.AddMinutes(extendedTime),
        //                     SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
        //                 };
        //                 return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        //             }
        //             else return "";
        //         }
        //         catch
        //         {
        //             return string.Empty;
        //         }
        //     }
    }
}