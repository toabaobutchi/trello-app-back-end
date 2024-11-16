using backend_apis.Utils;

namespace backend_apis.Controllers
{
    public class AppControllerBase : ProjectControllerBase
    {
        protected UserCookie GetUserCookie() => new(User);
        protected UserCookie UserCookie => GetUserCookie();
    }
}