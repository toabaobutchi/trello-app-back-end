namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record ContextResponse
    {
        public const string Owner = "owner";
        public const string Member = "member";
        public const string Admin = "admin";
        public const string Observer = "observer";
    }
}