using System.Collections.Generic;

namespace Unity.Entities.Editor
{
    class HierarchyItemsCache
    {
        readonly List<EntityHierarchyItem> m_CachedItems = new List<EntityHierarchyItem>(1024);
        IEnumerable<EntityHierarchyItem> m_RootItems;

        public void Rebuild(IEnumerable<EntityHierarchyItem> rootItems)
        {
            m_RootItems = rootItems;
            m_CachedItems.Clear();
            AppendAllItemsToCacheRecursively(m_RootItems);
        }

        public IEnumerable<EntityHierarchyItem> Items => m_CachedItems;

        void AppendAllItemsToCacheRecursively(IEnumerable<EntityHierarchyItem> itemsToAdd)
        {
            foreach (var item in itemsToAdd)
            {
                m_CachedItems.Add(item);

                // Forces the item to cache its lower case name
                // We want to do it in the prepare step because we can run it independently from the actual search
                item.GetCachedLowerCaseName();

                if (item.hasChildren)
                    AppendAllItemsToCacheRecursively(item.Children);
            }
        }
    }
}
