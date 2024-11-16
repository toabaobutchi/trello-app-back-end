using Google.Apis.Auth;

using GrantResult = (string AccessToken, backend_apis.Models.User User);

namespace backend_apis.Repositories
{
    public interface IAuthRepository
    {
        Task<GrantResult> CheckAndGrantAccessTokenAsync(GoogleJsonWebSignature.Payload payload);
    }
}