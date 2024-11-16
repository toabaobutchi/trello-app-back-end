namespace backend_apis.Exceptions
{
    public class EntityNotFoundException : ApiExpception
    {
        public EntityNotFoundException(string msg, EExceptionEntityType? entityType = null, string? reason = null) : base(msg, entityType, reason)
        {
        }
    }
}