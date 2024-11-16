namespace backend_apis.Utils
{
    public class CookieUtils
    {
        public static CookieOptions Create(DateTime expiredTime, string path = "/")
        {
            return new CookieOptions()
            {
                Expires = expiredTime,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Path = path
            };
        }
    }
}