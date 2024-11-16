using System.ComponentModel.DataAnnotations.Schema;

namespace backend_apis.Models
{
    public class Workspace
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public long CreatedAt { get; set; }
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }
        
        [ForeignKey(nameof(Owner))]
        public string OwnerId { get; set; } = null!;
        public User Owner { get; set; } = null!;
        public List<Project> Projects { get; set; } = new();
    }
}