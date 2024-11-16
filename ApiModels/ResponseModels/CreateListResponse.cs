using backend_apis.ApiModels.ResponseForBoardDisplay;
using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseModels
{
    public class CreateListResponse
    {
        public ListResponseForBoardDisplay CreatedList { get; set; } = null!;
        public string? ListOrder { get; set; }
        public static CreateListResponse Create(List list, string? listOrder = "") {
            return new CreateListResponse {
                CreatedList = ListResponseForBoardDisplay.Create(list),
                ListOrder = listOrder,
            };
        }
    }
}