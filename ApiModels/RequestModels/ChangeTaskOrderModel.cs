namespace backend_apis.ApiModels.RequestModels
{
    public class ChangeTaskOrderModel
    {
        public string NewListId { get; set; } = null!;
        public string OldListId { get; set; } = null!;
        public string NewTaskOrder { get; set; } = null!;
        public string OldTaskOrder { get; set; } = null!;
    }
}