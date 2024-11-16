namespace backend_apis.Exceptions
{
    public abstract class ApiExpception : Exception
    {
        public EExceptionEntityType EntityType { get; init; }
        public string? Reason { get; set; }
        public ApiExpception(string message, EExceptionEntityType? entityType = null, string? reason = null) : base(message)
        {
            EntityType = entityType ?? EExceptionEntityType.Undefined;
            Reason = reason;
        }

        public virtual object GetException()
        {
            return new { Message, Entity = EntityType.ToString(), Reason };
        }
    }
    public enum EExceptionEntityType
    {
        Workspace, Project, List, Assignment, Task, Subtask, ChangeLog, User, Comment, TaskAssignment, Attachment, Undefined
    }
}