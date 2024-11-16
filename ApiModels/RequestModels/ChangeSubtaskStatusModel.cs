namespace backend_apis.ApiModels.RequestModels
{
    public sealed record ChangeSubtaskStatusModel
    {
        public bool IsCompleted { get; set; }
    }
}