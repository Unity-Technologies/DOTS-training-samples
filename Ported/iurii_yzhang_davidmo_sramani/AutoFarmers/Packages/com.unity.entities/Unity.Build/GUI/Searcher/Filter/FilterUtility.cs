using System.Collections.Generic;

namespace Unity.Build
{
    static class FilterUtility
    {
        internal static AddComponentSearchFilter CreateAddComponentFilter(string input)
        {
            var names = new List<NameFilter>();
            GenerateFilterTokens(input, names);
            return new AddComponentSearchFilter(names);
        }

        static void GenerateFilterTokens(string filter, ICollection<NameFilter> names)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return;
            }

            filter = filter.Trim();

            var start = 0;
            for (var i = 0; i < filter.Length - 1; ++i)
            {
                if (filter[i] != ' ')
                {
                    continue;
                }
                names.Add(CreateNameFilter(filter.Substring(start, i - start)));
                start = i + 1;
            }

            if (start < filter.Length)
            { 
                names.Add(CreateNameFilter(filter.Substring(start)));
            }
        }

        static NameFilter CreateNameFilter(string input)
        {
            input = input.Trim();
            var filter = new NameFilter()
            {
                Name = input
            };
            return filter;
        }
    }
}
