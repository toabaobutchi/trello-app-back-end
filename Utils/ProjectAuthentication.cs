namespace backend_apis.Utils
{
    public sealed record ProjectAuthentication
    {
        public const string RequiredPolicy = "RequiredProjectAuthenticationPolicy";
        public const string AuthenticationScheme = "ProjectAuthenticationCookie";
        public const string HubPolicy = "ProjectHubAuthenticationPolicy";
    }
}