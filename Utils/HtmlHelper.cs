namespace backend_apis.Utils
{
    public class HtmlHelper
    {
        public static string EntityName(string name, string? additionalClassName = null)
        {
            if (!string.IsNullOrEmpty(additionalClassName))
            {
                return $@"<span class='entity-name {additionalClassName}'>{name}</span>";
            }
            return $@"<span class='entity-name'>{name}</span>";
        }
        public static string EntityNumber(int number, string? additionalClassName = null)
        {
            if (!string.IsNullOrEmpty(additionalClassName))
            {
                return $@"<span class='entity-number {additionalClassName}'>{number}</span>";
            }
            return $@"<span class='entity-number'>{number}</span>";
        }
    }
}