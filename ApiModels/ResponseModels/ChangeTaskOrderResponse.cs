using backend_apis.ApiModels.RequestModels;

namespace backend_apis.ApiModels.ResponseModels
{
    public class ChangeTaskOrderResponse
    {
        public string UpdatedNewListId { get; set; } = null!;
        public string UpdatedOldListId { get; set; } = null!;
        public string UpdatedNewTaskOrder { get; set; } = null!;
        public string UpdatedOldTaskOrder { get; set; } = null!;

        public static ChangeTaskOrderResponse Create(ChangeTaskOrderModel model)
        {
            return new ChangeTaskOrderResponse
            {
                UpdatedOldListId = model.OldListId,
                UpdatedOldTaskOrder = model.OldTaskOrder,
                UpdatedNewTaskOrder = model.NewTaskOrder,
                UpdatedNewListId = model.NewListId,
            }
            ;
        }
    }
}