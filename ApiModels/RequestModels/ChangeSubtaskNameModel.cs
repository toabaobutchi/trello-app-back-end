namespace backend_apis.ApiModels.RequestModels
{
    public sealed record ChangeSubtaskNameModel
    {
        public string Name { get; set; } = null!;
    }
}