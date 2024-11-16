using backend_apis.Utils;

namespace backend_apis.ApiModels.RequestModels
{
    public class CreateTaskModel
    {
        public string Name { get; set; } = null!;
        public string ListId { get; set; } = null!;

        public Models.Task ToTask(string? creatorId = null)
        {
            return new Models.Task()
            {
                Id = Guid.NewGuid(),
                Name = Name,
                ListId = ListId,
                CreatedAt = DateTimeUtils.GetSeconds(),
                Priority = null,
                CreatorId = creatorId ?? ""
            };
        }
    }
}