using System.Collections.Generic;
using System.Text;

namespace Unity.Entities.CodeGen
{
    internal static class ListExtensions
    {
        public static void Add<T>(this List<T> list, IEnumerable<T> elementsToAdd) => list.AddRange(elementsToAdd);
    }

    internal static class StringExtensions
    {
        public static string SeparateBy(this IEnumerable<string> elements, string delimiter)
        {
            bool first = true;
            var sb = new StringBuilder();
            foreach (var e in elements)
            {
                if (!first)
                    sb.Append(delimiter);
                sb.Append(e);
                first = false;
            }

            return sb.ToString();
        }

        public static string SeparateBySpace(this IEnumerable<string> elements) => elements.SeparateBy(" ");
        public static string SeparateByComma(this IEnumerable<string> elements) => elements.SeparateBy(",");
    }
}