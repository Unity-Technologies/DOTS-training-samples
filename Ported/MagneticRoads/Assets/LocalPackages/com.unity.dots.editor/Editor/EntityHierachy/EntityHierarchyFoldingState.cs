using System.Collections.Generic;

namespace Unity.Entities.Editor
{
    class EntityHierarchyFoldingState
    {
        readonly Dictionary<TreeViewItemStateKey, bool> m_FoldingState;

        public EntityHierarchyFoldingState(string key)
        {
            m_FoldingState = Unity.Serialization.Editor.SessionState<Dictionary<TreeViewItemStateKey, bool>>.GetOrCreate($"{typeof(EntityHierarchyFoldingState).FullName}+{key}");
        }

        public void OnFoldingStateChanged(in EntityHierarchyNodeId nodeId, bool isExpanded)
        {
            if (nodeId.Kind != NodeKind.Scene && nodeId.Kind != NodeKind.SubScene)
                return;

            m_FoldingState[nodeId] = isExpanded;
        }

        public bool? GetFoldingState(in EntityHierarchyNodeId nodeId)
            => m_FoldingState.TryGetValue(nodeId, out var isExpanded) ? isExpanded : (bool?)null;

        internal struct TreeViewItemStateKey
        {
            public NodeKind Kind;
            public int Id;

            public static implicit operator TreeViewItemStateKey(in EntityHierarchyNodeId nodeId)
                => new TreeViewItemStateKey { Kind = nodeId.Kind, Id = nodeId.Id };
        }
    }
}
