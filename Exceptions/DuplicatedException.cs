namespace backend_apis.Exceptions
{
    public class DuplicatedException : Exception
    {
        public DuplicatedException() : base()
        {
        }
        public DuplicatedException(string msg) : base(msg) { }
    }
}