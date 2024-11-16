namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record ReferenceTasks
    {
        public List<RelatedTaskResponse> Dependencies { get; set; } = [];
        public List<RelatedTaskResponse> ChildTasks { get; set; } = [];
    }
}