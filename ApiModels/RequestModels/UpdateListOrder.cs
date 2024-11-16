namespace backend_apis.ApiModels.RequestModels
{
    public class UpdateListOrder
    {
        public string NewListOrder { get; set; } = null!;
        public string SubjectId { get; set; } = null!;
        public string ObjectId { get; set; } = null!;
    }
}