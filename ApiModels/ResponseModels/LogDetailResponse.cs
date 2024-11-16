namespace backend_apis.ApiModels.ResponseModels
{
    public class LogDetailResponse
    {
        public object EntityId { get; set; } = null!;
        public string DetailLog { get; set; } = null!;
        public string EntityType { get; set; } = null!;
        public string LogAction { get; set; } = null!;

        public static LogDetailResponse Create(LogDetail model)
        {
            return new LogDetailResponse
            {
                EntityId = model.EntityId,
                DetailLog = model.DetailLog,
                EntityType = model.EntityType.ToString(),
                LogAction = model.LogAction.ToString(),
            };
        }
    }
}