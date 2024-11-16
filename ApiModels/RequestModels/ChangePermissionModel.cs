
namespace backend_apis.ApiModels.RequestModels
{
    public sealed record ChangePermissionModel
    {
        public string Permission { get; set; } = null!;
        
    }
}