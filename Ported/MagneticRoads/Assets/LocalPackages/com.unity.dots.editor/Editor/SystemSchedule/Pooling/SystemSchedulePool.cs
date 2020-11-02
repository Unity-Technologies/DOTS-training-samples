using System.Collections.Generic;

namespace Unity.Entities.Editor
{
    static class SystemSchedulePool
    {
        static readonly Dictionary<SystemScheduleTreeView, HashSet<SystemInformationVisualElement>> k_PerTreeViewElements = new Dictionary<SystemScheduleTreeView, HashSet<SystemInformationVisualElement>>();

        public static SystemTreeViewItem GetSystemTreeViewItem(PlayerLoopSystemGraph graph, IPlayerLoopNode node, SystemTreeViewItem parent, World world)
        {
            var item = Pool<SystemTreeViewItem>.GetPooled(LifetimePolicy.Permanent);
            item.World = world;
            item.Graph = graph;
            item.Node = node;
            item.parent = parent;
            return item;
        }

        public static void ReturnToPool(SystemTreeViewItem item)
        {
            Pool<SystemTreeViewItem>.Release(item);
        }

        public static SystemInformationVisualElement GetSystemInformationVisualElement(SystemScheduleTreeView treeView)
        {
            var item = Pool<SystemInformationVisualElement>.GetPooled(LifetimePolicy.Permanent);
            if (!k_PerTreeViewElements.TryGetValue(treeView, out var list))
            {
                k_PerTreeViewElements[treeView] = list = new HashSet<SystemInformationVisualElement>();
            }

            list.Add(item);
            item.TreeView = treeView;
            return item;
        }

        public static void ReturnAllToPool(SystemScheduleTreeView treeView)
        {
            if (!k_PerTreeViewElements.TryGetValue(treeView, out var list))
                return;

            foreach (var item in list)
            {
                Pool<SystemInformationVisualElement>.Release(item);
            }
            list.Clear();
        }

        public static void ReturnToPool(SystemScheduleTreeView treeView, SystemInformationVisualElement item)
        {
            if (!k_PerTreeViewElements.TryGetValue(treeView, out var list))
                return;

            if (list.Remove(item))
                Pool<SystemInformationVisualElement>.Release(item);
        }
    }
}
