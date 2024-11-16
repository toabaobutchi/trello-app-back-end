namespace backend_apis.ApiModels.RequestModels
{
    public sealed record DuplicateTaskModel
    {
        public bool? InheritPriority { get; set; }
        public bool? InheritDescription { get; set; }
        public bool? InheritDueDate { get; set; }
        public int DuplicateTaskCount { get; set; }
        public string ListId { get; set; } = null!;
    }
}