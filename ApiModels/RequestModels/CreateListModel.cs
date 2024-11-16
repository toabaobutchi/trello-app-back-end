using backend_apis.Models;
using backend_apis.Utils;

namespace backend_apis.ApiModels.RequestModels
{
    public class CreateListModel
    {
        public string Name { get; set; } = null!;
        public string ProjectId { get; set; } = null!;

        public List ToList()
        {
            return new List()
            {
                Id = Guid.NewGuid().ToString(),
                CreatedAt = DateTimeUtils.GetSeconds(),
                ProjectId = ProjectId,
                Name = Name
            };
        }
    }
}