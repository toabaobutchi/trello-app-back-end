using backend_apis.Models;

namespace backend_apis.ApiModels.ResponseModels
{
    public class OwnerInfo
    {
        public string Id { get; set; } = null!;
        public string? DisplayName { get; set; }
        public string Email { get; set; } = null!;
        public string? Avatar { get; set; }
        public static OwnerInfo Create(User user)
        {
            return new OwnerInfo
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Avatar = user.Avatar
            };
        }
    }
    public class WorkspaceResponseWithRelatedProjects : ContextBase
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Slug { get; set; } = null!;
        public long CreatedAt { get; set; }
        public string OwnerId { get; set; } = null!;
        public OwnerInfo? Owner { get; set; } = null!;
        public List<ProjectContextModel> Projects { get; set; } = new();
        public static WorkspaceResponseWithRelatedProjects Create(Workspace workspace, string context, string uid)
        {
            return new WorkspaceResponseWithRelatedProjects
            {
                Id = workspace.Id,
                Name = workspace.Name,
                Description = workspace.Description,
                Slug = workspace.Slug,
                CreatedAt = workspace.CreatedAt,
                OwnerId = workspace.OwnerId,
                Context = context,
                Projects = workspace.Projects.Select(p => ProjectContextModel.Create(p, context, uid)).ToList()
            };
        }
    }
    public class ProjectContextModel : ContextBase
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Slug { get; set; } = null!;
        public long CreatedAt { get; set; }
        public int MemberCount { get; set; }
        public string? Color { get; set; }
        public long? DueDate { get; set; }
        public AssignmentConfig? AssignmentConfig { get; set; }
        public static ProjectContextModel Create(Project project, string context, string uid)
        {
            return new ProjectContextModel()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Slug = project.Slug,
                CreatedAt = project.CreatedAt,
                DueDate = project.DueDate,
                MemberCount = project.Assignments.Count,
                Color = project.Color,
                Context = context,
                AssignmentConfig = AssignmentConfig.Create(project.Assignments.FirstOrDefault(a => a.UserId == uid))
            };
        }
    }
    public class AssignmentConfig
    {
        public string Id { get; set; } = null!;
        public bool? IsPinned { get; set; }
        public long? LastViewAt { get; set; }
        public string? Permission { get; set; }
        public string ProjectId { get; set; } = null!;

        public static AssignmentConfig Create(Assignment assignment)
        {
            return new AssignmentConfig()
            {
                Id = assignment.Id,
                IsPinned = assignment.isPinned,
                LastViewAt = assignment.LastViewAt,
                Permission = assignment.Permission.ToString(),
                ProjectId = assignment.ProjectId
            };
        }
    }
}