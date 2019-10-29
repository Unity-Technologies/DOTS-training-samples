using System.Collections.Generic;
using System.Linq;

namespace Unity.Build
{
    sealed class AddComponentSearchFilter
    {
        readonly NameFilter[] m_Names;

        public AddComponentSearchFilter(List<NameFilter> names)
        {
            m_Names = names.ToArray();
        }

        public bool Keep(string name)
        {
            return m_Names.All(filter => filter.Keep(name));
        }
    }
}