namespace backend_apis.Utils
{
    public sealed record WorkspaceAuthentication
    {
        public const string AuthenticationScheme = "WorkspaceAuthenticationCookie";
        public const string RequiredPolicy = "RequiredWorkspaceAuthenticationPolicy";
        // public const string MemberPolicy = "MemberOfWorkspacePolicy";
    }
}