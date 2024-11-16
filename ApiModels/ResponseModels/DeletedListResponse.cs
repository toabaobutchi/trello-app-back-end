using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseModels
{
    public sealed record DeletedListResponse
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string ProjectId { get; set; } = null!;

        public static DeletedListResponse Create(in List list)
        {
            return new DeletedListResponse
            {
                Id = list.Id,
                Name = list.Name,
                ProjectId = list.ProjectId
            };
        }
    }
}