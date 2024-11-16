using backend_apis.Models;

namespace backend_apis.ApiModels.RequestModels
{
    public sealed record UpdateListModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? WipLimit { get; set; }

        public void Update(ref List list)
        {
            list.Name = Name ?? list.Name;
            list.Description = Description ?? list.Description;
            list.WipLimit = WipLimit ?? list.WipLimit;
        }
    }
}