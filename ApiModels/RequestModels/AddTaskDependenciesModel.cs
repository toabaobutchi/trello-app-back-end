namespace backend_apis.ApiModels.RequestModels
{
    public sealed record AddTaskDependenciesModel
    {
        public IEnumerable<string> Dependencies { get; set; } = null!;
    }
}