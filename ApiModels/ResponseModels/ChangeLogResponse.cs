using backend_apis.Utils;

namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record ChangeLogResponse
    {
        public int Id { get; set; }
        public string ProjectId { get; set; } = null!;
        public string? AssignmentId { get; set; }
        public LogDetailResponse Log { get; set; } = null!;
        public long CreatedAt { get; set; }

        public static ChangeLogResponse Create(Models.ChangeLog changeLog)
        {
            var logDetail = JSON.Parse<LogDetail>(changeLog.Log);
            return new ChangeLogResponse
            {
                Id = changeLog.Id,
                ProjectId = changeLog.ProjectId,
                AssignmentId = changeLog.AssignmentId,
                Log = LogDetailResponse.Create(logDetail),
                CreatedAt = changeLog.CreatedAt
            };
        }
    }
}