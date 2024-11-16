using System.Text.Json;
using System.Text.Json.Serialization;

namespace backend_apis.Utils
{
    public class JSON
    {
        public static string Stringify<T>(T value)
        {
            return JsonSerializer.Serialize(value, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            });
        }
        public static T? Parse<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}