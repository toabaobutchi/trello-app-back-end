namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record DeletedRelationshipResponse
    {
        public string TaskId { get; set; } = null!;
        public string RelationshipId { get; set; } = null!;
        public string RelationshipType { get; set; } = null!;

        public static DeletedRelationshipResponse Create(Models.TaskDependenceDetail dependenceDetail, string type)
        {
            if (type == DeletedRelationshipResponseType.Dependencies)
            {
                return new DeletedRelationshipResponse()
                {
                    TaskId = dependenceDetail.TaskId.ToString(),
                    RelationshipId = dependenceDetail.DependentTaskId.ToString(),
                    RelationshipType = type,
                };
            }
            else {
                return new DeletedRelationshipResponse()
                {
                    TaskId = dependenceDetail.DependentTaskId.ToString(),
                    RelationshipId = dependenceDetail.TaskId.ToString(),
                    RelationshipType = type,
                };
            }
        }
    }
    public sealed record DeletedRelationshipResponseType
    {
        public const string Dependencies = "Dependencies";
        public const string Children = "Children";
    }
}