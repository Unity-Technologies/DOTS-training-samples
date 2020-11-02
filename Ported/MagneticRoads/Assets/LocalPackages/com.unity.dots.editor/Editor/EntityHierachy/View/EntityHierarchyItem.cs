using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Editor.Bridge;
using UnityEditor;

namespace Unity.Entities.Editor
{
    class EntityHierarchyItem : ITreeViewItem, IPoolable
    {
        static readonly string k_ChildrenListModificationExceptionMessage =
            L10n.Tr($"{nameof(EntityHierarchyItem)} does not allow external modifications to its list of children.");

        readonly List<EntityHierarchyItem> m_Children = new List<EntityHierarchyItem>();
        bool m_ChildrenInitialized;

        // Caching name and pre-lowercased name to speed-up search
        string m_CachedName;

        // Public for binding with SearchElement
        // ReSharper disable once InconsistentNaming
        public string m_CachedLowerCaseName;

        IEntityHierarchy m_EntityHierarchy;

        public void Initialize(ITreeViewItem parentItem, in EntityHierarchyNodeId nodeId, IEntityHierarchy entityHierarchy)
        {
            parent = parentItem;
            NodeId = nodeId;
            m_EntityHierarchy = entityHierarchy;
        }

        public EntityHierarchyNodeId NodeId { get; private set; }

        public IEntityHierarchyState HierarchyState => m_EntityHierarchy.State;

        public World World => m_EntityHierarchy.World;

        public List<EntityHierarchyItem> Children
        {
            get
            {
                if (!m_ChildrenInitialized)
                {
                    PopulateChildren();
                    m_ChildrenInitialized = true;
                }
                return m_Children;
            }
        }

        public string CachedName => m_CachedName ?? (m_CachedName = HierarchyState.GetNodeName(NodeId));

        public string GetCachedLowerCaseName() => m_CachedLowerCaseName ?? (m_CachedLowerCaseName = CachedName.ToLowerInvariant());

        public int id => NodeId.GetHashCode();

        public ITreeViewItem parent { get; private set; }

        IEnumerable<ITreeViewItem> ITreeViewItem.children => Children;

        public bool hasChildren => HierarchyState.HasChildren(NodeId);

        void ITreeViewItem.AddChild(ITreeViewItem _) => throw new NotSupportedException(k_ChildrenListModificationExceptionMessage);

        void ITreeViewItem.AddChildren(IList<ITreeViewItem> _) => throw new NotSupportedException(k_ChildrenListModificationExceptionMessage);

        void ITreeViewItem.RemoveChild(ITreeViewItem _) => throw new NotSupportedException(k_ChildrenListModificationExceptionMessage);

        void IPoolable.Reset()
        {
            NodeId = default;

            m_CachedName = null;
            m_CachedLowerCaseName = null;
            m_EntityHierarchy = null;
            parent = null;
            m_Children.Clear();
            m_ChildrenInitialized = false;
        }

        void IPoolable.ReturnToPool()
        {
            foreach (var child in m_Children)
                ((IPoolable)child).ReturnToPool();

            EntityHierarchyPool.ReturnTreeViewItem(this);
        }

        void PopulateChildren()
        {
            using (var childNodes = HierarchyState.GetChildren(NodeId, Allocator.TempJob))
            {
                foreach (var node in childNodes)
                {
                    var item = EntityHierarchyPool.GetTreeViewItem(this, node, m_EntityHierarchy);
                    m_Children.Add(item);
                }
            }
        }
    }
}
