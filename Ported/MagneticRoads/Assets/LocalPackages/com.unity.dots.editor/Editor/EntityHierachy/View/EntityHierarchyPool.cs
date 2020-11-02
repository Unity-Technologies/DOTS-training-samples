using System.Collections.Generic;
using Unity.Editor.Bridge;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    static class EntityHierarchyPool
    {
        static readonly Dictionary<VisualElement, HashSet<EntityHierarchyItemView>> k_PerTreeViewElements =
            new Dictionary<VisualElement, HashSet<EntityHierarchyItemView>>();

        public static EntityHierarchyItemView GetVisualElement(VisualElement owner)
        {
            var item = Pool<EntityHierarchyItemView>.GetPooled();
            if (!k_PerTreeViewElements.TryGetValue(owner, out var list))
                k_PerTreeViewElements[owner] = list = new HashSet<EntityHierarchyItemView>();

            list.Add(item);
            item.Owner = owner;
            return item;
        }

        public static void ReturnAllVisualElements(VisualElement owner)
        {
            if (!k_PerTreeViewElements.TryGetValue(owner, out var list))
                return;

            foreach (var item in list)
                Pool<EntityHierarchyItemView>.Release(item);

            list.Clear();
        }

        public static void ReturnVisualElement(EntityHierarchyItemView item)
        {
            if (!k_PerTreeViewElements.TryGetValue(item.Owner, out var list))
                return;

            if (list.Remove(item))
                Pool<EntityHierarchyItemView>.Release(item);
        }

        public static EntityHierarchyItem GetTreeViewItem(ITreeViewItem parent, EntityHierarchyNodeId nodeId, IEntityHierarchy hierarchy)
        {
            var item = Pool<EntityHierarchyItem>.GetPooled();
            item.Initialize(parent, nodeId, hierarchy);
            return item;
        }

        public static void ReturnTreeViewItem(EntityHierarchyItem item)
        {
            Pool<EntityHierarchyItem>.Release(item);
        }
    }
}
