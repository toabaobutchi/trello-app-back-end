namespace backend_apis.ApiModels.RequestModels
{
    public sealed record UpdateProjectModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public long? DueDate { get; set; }
        public string? Color { get; set; }
    }
}