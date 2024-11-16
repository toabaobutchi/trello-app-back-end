namespace backend_apis.Utils
{
    public sealed record AppSettings
    {
        public const string AccessTokenSecretKey = "AppSettings:AccessTokenSecretKey";
        public const string RefreshTokenSecretKey = "AppSettings:RefreshTokenSecretKey";
        public const string GoogleClientId = "AppSettings:GoogleClientId";
    }
}