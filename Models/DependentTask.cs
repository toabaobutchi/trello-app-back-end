using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Models
{
  [PrimaryKey(nameof(TaskId), nameof(DependentTaskId))]
  public class TaskDependenceDetail
  {
    // đang đề cập đến task nào
    [ForeignKey(nameof(Task))]
    public Guid TaskId { get; set; }

    // phụ thuộc vào task nào
    [ForeignKey(nameof(DependentTask))]
    public Guid DependentTaskId { get; set; }

    // reference
    public Task Task { get; set; } = null!; // tham chieu den `ChildDependentTasks`
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Task DependentTask { get; set; } = null!; // tham chieu den `ParentDependentTasks`
  }
}