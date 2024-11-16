namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record AuthResponse
    {
        public string AccessToken { get; set; } = null!;
        public UserResponse? AccountInfo { get; set; }
        public AuthResponse() { }
        public AuthResponse(string accessToken, UserResponse? accountInfo)
        {
            AccessToken = accessToken;
            AccountInfo = accountInfo;
        }
        public AuthResponse(string accessToken, Models.User user)
        {
            AccessToken = accessToken;
            AccountInfo = UserResponse.Create(user);
        }
    }
}