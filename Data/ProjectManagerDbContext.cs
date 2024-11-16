using backend_apis.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_apis.Data
{
    public class ProjectManagerDbContext : DbContext
    {
        public ProjectManagerDbContext()
        {
        }
        public ProjectManagerDbContext(DbContextOptions<ProjectManagerDbContext> options) : base(options)
        {
        }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Workspace> Workspaces { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectInvitation> ProjectInvitations { get; set; }
        public virtual DbSet<List> Lists { get; set; }
        public virtual DbSet<Assignment> Assignments { get; set; }
        public virtual DbSet<Models.Task> Tasks { get; set; }
        public virtual DbSet<ChangeLog> ChangeLogs { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<TaskAssignment> TaskAssignments { get; set; }
        public virtual DbSet<SubTask> SubTasks { get; set; }
        public virtual DbSet<HubConnection> HubConnections { get; set; }
        public virtual DbSet<ProjectHubConnection> ProjectHubConnections { get; set; }
        public virtual DbSet<ProjectComment> ProjectComments { get; set; }
        public virtual DbSet<TaskDependenceDetail> TaskDependenceDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskAssignment>(taskAssignment =>
            {
                taskAssignment.HasOne(ta => ta.Assigner)
                            .WithMany(a => a.TaskAssigners)
                            .HasForeignKey(ta => ta.AssignerId);
            });
            modelBuilder.Entity<TaskDependenceDetail>(depTask =>
            {
                depTask.HasOne(dt => dt.DependentTask)
                        .WithMany(t => t.ChildDependentTasks)
                        .HasForeignKey(dt => dt.DependentTaskId);
                depTask.HasOne(dt => dt.Task)
                        .WithMany(t => t.ParentDependentTasks)
                        .HasForeignKey(dt => dt.TaskId);
            });
            modelBuilder.Entity<User>(user =>
            {
                user.HasIndex(u => u.Email).IsUnique();
            });
            modelBuilder.Entity<Workspace>(workspace =>
            {
                workspace.HasIndex(w => w.Slug).IsUnique();
            });
            modelBuilder.Entity<Project>(project =>
            {
                project.HasIndex(p => p.Slug).IsUnique();
            });
            modelBuilder.Entity<Models.Task>(task =>
            {
                task.HasOne(t => t.Deleter).WithMany(a => a.DeleteTasks).HasForeignKey(t => t.DeleterId);
            });
            modelBuilder.Entity<SubTask>(subtask =>
            {
                subtask.HasOne(s => s.Assigner)
                    .WithMany(a => a.AssignSubtasks)
                    .HasForeignKey(s => s.AssignerId);
            });

            // modelBuilder.Entity<TaskAssignment>(ta =>
            // {
            //     ta.HasKey("TaskId", "AssignmentId");
            // });
        }
    }
}