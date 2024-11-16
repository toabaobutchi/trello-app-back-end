using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseModels
{
    public class ListChangeResponse
    {
        public string ListId { get; set; } = null!;
        public int UpdatedIndex { get; set; }
        public long UpdatedAt { get; set; }

        public static ListChangeResponse Create(List list) {
            return new ListChangeResponse
            {
                ListId = list.Id
            };
        }
    }
}