using System.Collections.Generic;

namespace Unity.Entities.Editor
{
    static class SearchUtility
    {
        public static IEnumerable<string> SplitSearchStringBySpace(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                yield break;

            searchString = searchString.Trim();

            if (!searchString.Contains(" "))
            {
                yield return searchString;
                yield break;
            }

            searchString = searchString.Replace(": ", ":");

            foreach (var singleString in searchString.Split(' '))
            {
                yield return singleString;
            }
        }
    }
}
