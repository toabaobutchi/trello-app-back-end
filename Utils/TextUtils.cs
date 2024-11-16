using System.Text;
using System.Globalization;

namespace backend_apis.Utils
{
    public class TextUtils
    {
        public static string CreateSlug(string name)
        {
            // Step 1: Remove diacritics
            string normalized = RemoveDiacritics(name);

            // Step 2: Replace spaces with dashes
            string dashed = normalized.Replace(' ', '-');

            // Step 3: Convert to lowercase
            string lowercase = dashed.ToLower();

            return lowercase;
        }
        private static string RemoveDiacritics(string text)
        {
            string normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        public static string CreateId()
        {
            var guid = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(guid).Replace("=", "").Replace("+", "-").Replace("/", "_");
        }
        public static string CreateAssignmentId(string pid, string uid)
        {
            var combineByteArray = Encoding.UTF8.GetBytes($"{pid}-{uid}");
            return Convert.ToBase64String(combineByteArray).Replace("=", "").Replace("+", "-").Replace("/", "_");
        }
    }
}