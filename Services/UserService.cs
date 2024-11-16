using backend_apis.Utils;
using Google.Apis.Auth;

namespace backend_apis.Services
{
    public class UserService
    {
        public Models.User CreateUserFromPayLoad(GoogleJsonWebSignature.Payload payload)
        {
            return new Models.User()
            {
                Id = payload.Subject,
                Email = payload.Email,
                DisplayName = payload.Name,
                Avatar = payload.Picture,
                JoinAt = DateTimeUtils.GetSeconds(),
            };
        }
    }
}