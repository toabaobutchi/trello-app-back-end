using System.ComponentModel.DataAnnotations;

namespace backend_apis.Models
{
    public class HubConnection
    {
        [Key]
        public string UserId { get; set; } = null!;
        public string ConnectionId { get; set; } = null!;
    }
}