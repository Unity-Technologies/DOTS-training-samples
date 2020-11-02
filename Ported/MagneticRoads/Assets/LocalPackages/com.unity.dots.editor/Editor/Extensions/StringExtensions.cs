using System.IO;
using System.Text.RegularExpressions;

namespace Unity.Entities.Editor
{
    static class StringExtensions
    {
        private static readonly Regex s_ToWordRegex = new Regex(@"[^\w]", RegexOptions.Compiled);

        public static string SingleQuoted(this string value)
        {
            return $"'{value.Trim('\'')}'";
        }

        public static string DoubleQuoted(this string value)
        {
            return $"\"{value.Trim('\"')}\"";
        }

        public static string ToHyperLink(this string value, string key = null)
        {
            return string.IsNullOrEmpty(key) ? $"<a>{value}</a>" : $"<a {key}={value.DoubleQuoted()}>{value}</a>";
        }

        public static string ToIdentifier(this string value)
        {
            return s_ToWordRegex.Replace(value, "_");
        }

        public static string ToForwardSlash(this string value)
        {
            return value.Replace('\\', Path.AltDirectorySeparatorChar);
        }
    }
}