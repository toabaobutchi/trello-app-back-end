namespace backend_apis.Utils
{
    public sealed record ProjectInvitationEmail
    {
        public string InvitedEmail { get; set; } = null!;
    }
}