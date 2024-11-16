namespace backend_apis.Utils
{
    public sealed record DateTimeUtils
    {
        public static long GetSeconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}