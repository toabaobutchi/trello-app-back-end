using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record UpdatedListResponse
    {
        public string Id { get; set; } = null!;
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? WipLimit { get; set; }
        public static UpdatedListResponse Create(in List list)
        {
            return new UpdatedListResponse()
            {
                Id = list.Id,
                Name = list.Name,
                Description = list.Description,
                WipLimit = list.WipLimit
            };
        }
    }
}