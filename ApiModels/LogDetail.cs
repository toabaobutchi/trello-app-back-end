using backend_apis.Utils;

namespace backend_apis.ApiModels
{
    public sealed record LogDetail
    {
        public object? EntityId { get; set; } = null!;
        public string DetailLog { get; set; } = null!;
        public EEntityType EntityType { get; set; }
        public ELogAction LogAction { get; set; }

        public static LogDetail Create(object? entityId, string detailLog, ELogAction logAction, EEntityType type)
        {
            return new LogDetail
            {
                EntityId = entityId,
                DetailLog = detailLog,
                LogAction = logAction,
                EntityType = type,
            };
        }
        public override string ToString()
        {
            return JSON.Stringify(this);
        }
        public static LogDetail? Parse(string json)
        {
            return JSON.Parse<LogDetail>(json);
        }
    }

    public enum ELogAction
    {
        Create,
        Update,
        Delete,
        Join,
        Unassign,
        Assign
    }
    public enum EEntityType
    {
        Project,
        Task,
        List,
        Subtask,
        Assignment,
        Invitation,
        TaskAssignment
    }
}