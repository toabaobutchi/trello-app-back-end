namespace backend_apis.ApiModels.ResponseModels
{
    public class ChangePermissionResponse
    {
        public string AssignmentId { get; set; } = null!;
        public string NewPermission { get; set; } = null!;
    }
}