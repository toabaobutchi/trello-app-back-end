namespace backend_apis.Exceptions
{
    public class OverDueDateException : Exception
    {
        public OverDueDateException()
        : base()
        {
        }

        public OverDueDateException(string message) : base(message)
        {
        }
    }
}