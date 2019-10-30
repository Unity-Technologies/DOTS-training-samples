namespace Unity.Serialization.Json
{
    static class StringExtensions
    {
        internal static string ToForwardSlash(this string value)
        {
            return value.Replace('\\', '/');
        }
    }
}
