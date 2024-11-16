namespace backend_apis.ApiModels.ResponseModels
{
    public class ApiResponse
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}