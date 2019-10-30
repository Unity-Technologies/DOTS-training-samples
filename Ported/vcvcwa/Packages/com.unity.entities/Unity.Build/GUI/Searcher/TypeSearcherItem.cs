using System;
using System.Collections.Generic;
using UnityEditor.Searcher;

namespace Unity.Build
{
    sealed class TypeSearcherItem : SearcherItem
    {
        public Type Type { get; }

        public TypeSearcherItem(Type type, string displayName = "", string help = "", List<SearcherItem> children = null)
            : base(string.IsNullOrEmpty(displayName) ? type.Name : displayName, help, children)
        {
            Type = type;
        }
    }
}