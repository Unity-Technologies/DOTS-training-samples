using System;

namespace Unity.Build
{
    struct NameFilter
    {
        public string Name { get; set; }

        public bool Keep(string name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return true;
            }

            return name.IndexOf(Name, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}