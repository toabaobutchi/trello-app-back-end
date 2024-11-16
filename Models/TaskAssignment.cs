using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Models
{
    public class TaskAssignment
    {
        public string Id { get; set; } = null!;

        [ForeignKey(nameof(Task))]
        public Guid TaskId { get; set; }
        public long AssignedAt { get; set; }

        [ForeignKey(nameof(Assigner))]
        public string? AssignerId { get; set; }

        [ForeignKey(nameof(Assignment))]
        public string AssignmentId { get; set; } = null!;
        public Assignment Assignment { get; set; } = null!;

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Task Task { get; set; } = null!;
        public Assignment? Assigner { get; set; }
    }
}