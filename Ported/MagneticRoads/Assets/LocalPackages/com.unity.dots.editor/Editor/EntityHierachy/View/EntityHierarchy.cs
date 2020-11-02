using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Collections;
using Unity.Editor.Bridge;
using Unity.Properties.UI;
using UnityEditor;
using UnityEngine.UIElements;
using UnityObject = UnityEngine.Object;

namespace Unity.Entities.Editor
{
    class EntityHierarchy : VisualElement, IDisposable
    {
        enum ViewMode { Uninitialized, Full, Search }

        internal static readonly string ComponentTypeNotFoundTitle = L10n.Tr("Type not found");
        internal static readonly string ComponentTypeNotFoundContent = L10n.Tr("\"{0}\" is not a component type");
        internal static readonly string NoEntitiesFoundTitle = L10n.Tr("No entity matches your search");

        readonly int[] m_CachedSingleSelectionBuffer = new int[1];

        readonly List<ITreeViewItem> m_TreeViewRootItems = new List<ITreeViewItem>(128);
        readonly List<int> m_TreeViewItemsToExpand = new List<int>(128);
        readonly List<EntityHierarchyItem> m_ListViewFilteredItems = new List<EntityHierarchyItem>(1024);
        readonly EntityHierarchyFoldingState m_EntityHierarchyFoldingState;
        readonly VisualElement m_ViewContainer;
        readonly TreeView m_TreeView;
        readonly ListView m_ListView;
        readonly HierarchyItemsCache m_ItemsCache;
        readonly CenteredMessageElement m_SearchEmptyMessage;

        ViewMode m_ViewMode = ViewMode.Uninitialized;
        IEntityHierarchy m_Hierarchy;
        EntityHierarchyQueryBuilder.Result m_QueryBuilderResult;
        bool m_SearcherCacheNeedsRebuild = true;
        bool m_StructureChanged;
        uint m_RootVersion;
        ISearchQuery<EntityHierarchyItem> m_CurrentQuery;

        public EntityHierarchy(EntityHierarchyFoldingState entityHierarchyFoldingState)
        {
            m_EntityHierarchyFoldingState = entityHierarchyFoldingState;

            style.flexGrow = 1.0f;
            m_ViewContainer = new VisualElement();
            m_ViewContainer.style.flexGrow = 1.0f;
            m_TreeView = new TreeView(m_TreeViewRootItems, Constants.ListView.ItemHeight, MakeTreeViewItem, BindTreeViewItem)
            {
                selectionType = SelectionType.Single,
                name = Constants.EntityHierarchy.FullViewName
            };
            m_TreeView.style.flexGrow = 1.0f;
            m_TreeView.onSelectionChange += OnLocalSelectionChanged;
            m_TreeView.Q<ListView>().RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == (int)MouseButton.LeftMouse)
                    Selection.activeObject = null;
            });
            m_TreeView.ItemExpandedStateChanging += (item, isExpanding) =>
            {
                var entityHierarchyItem = (EntityHierarchyItem)item;
                if (entityHierarchyItem.NodeId.Kind == NodeKind.Scene || entityHierarchyItem.NodeId.Kind == NodeKind.SubScene)
                    m_EntityHierarchyFoldingState.OnFoldingStateChanged(entityHierarchyItem.NodeId, isExpanding);
            };

            m_ListView = new ListView(m_ListViewFilteredItems, Constants.ListView.ItemHeight, MakeListViewItem, BindListViewItem)
            {
                selectionType = SelectionType.Single,
                name = Constants.EntityHierarchy.SearchViewName
            };
            m_ListView.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button == (int)MouseButton.LeftMouse)
                    Selection.activeObject = null;
            });

            m_ListView.style.flexGrow = 1.0f;

            m_SearchEmptyMessage = new CenteredMessageElement();
            m_SearchEmptyMessage.Hide();
            Add(m_SearchEmptyMessage);

#if UNITY_2020_1_OR_NEWER
            m_ListView.onSelectionChange += OnLocalSelectionChanged;
#else
            m_ListView.onSelectionChanged += OnSelectionChanged;
#endif

            m_ItemsCache = new HierarchyItemsCache();

            SwitchViewMode(ViewMode.Full);

            Add(m_ViewContainer);
            Selection.selectionChanged += OnGlobalSelectionChanged;
        }

        VisualElement CurrentView
        {
            get
            {
                switch (m_ViewMode)
                {
                    case ViewMode.Full:
                        return m_TreeView;
                    case ViewMode.Search:
                        return m_ListView;
                    default:
                        return null;
                }
            }
        }

        public void Dispose()
        {
            // ReSharper disable once DelegateSubtraction
            Selection.selectionChanged -= OnGlobalSelectionChanged;
        }

        public void Select(int id)
        {
            switch (m_ViewMode)
            {
                case ViewMode.Full:
                {
                    m_TreeView.Select(id, false);
                    break;
                }
                case ViewMode.Search:
                {
                    var index = m_ListViewFilteredItems.FindIndex(item => item.NodeId.GetHashCode() == id);
                    if (index != -1)
                    {
                        m_ListView.ScrollToItem(index);
                        m_CachedSingleSelectionBuffer[0] = index;
                        m_ListView.SetSelectionWithoutNotify(m_CachedSingleSelectionBuffer);
                    }

                    break;
                }
            }
        }

        public void Deselect()
        {
            m_TreeView.ClearSelection();
            m_ListView.ClearSelection();
        }

        public void SetFilter(ISearchQuery<EntityHierarchyItem> searchQuery, EntityHierarchyQueryBuilder.Result queryBuilderResult)
        {
            m_QueryBuilderResult = queryBuilderResult;
            m_SearchEmptyMessage.ToggleVisibility(!queryBuilderResult.IsValid);
            m_ViewContainer.ToggleVisibility(queryBuilderResult.IsValid);

            if (!queryBuilderResult.IsValid)
            {
                m_SearchEmptyMessage.Title = ComponentTypeNotFoundTitle;
                m_SearchEmptyMessage.Message = string.Format(ComponentTypeNotFoundContent, queryBuilderResult.ErrorComponentType);
                return;
            }

            m_CurrentQuery = searchQuery;
            var showFilterView = queryBuilderResult.QueryDesc != null || m_CurrentQuery != null && !string.IsNullOrWhiteSpace(m_CurrentQuery.SearchString) && m_CurrentQuery.Tokens.Count != 0;
            if (showFilterView && m_ViewMode != ViewMode.Search)
            {
                SwitchViewMode(ViewMode.Search);
                return;
            }

            if (!showFilterView && m_ViewMode == ViewMode.Search)
            {
                SwitchViewMode(ViewMode.Full);
                return;
            }

            RefreshView();
        }

        public void Refresh(IEntityHierarchy entityHierarchy)
        {
            if (m_Hierarchy == entityHierarchy)
                return;

            m_Hierarchy = entityHierarchy;
            UpdateStructure();
            OnUpdate();
        }

        public new void Clear()
        {
            m_TreeViewRootItems.Clear();
            m_ListViewFilteredItems.Clear();
            m_ListView.Refresh();
            m_TreeView.Refresh();
        }

        public void UpdateStructure()
        {
            // Topology changes will be applied during the next update
            m_StructureChanged = true;
            m_SearcherCacheNeedsRebuild = true;
            m_RootVersion = 0;
        }

        public void OnUpdate()
        {
            if (m_Hierarchy?.GroupingStrategy == null)
                return;

            var rootVersion = m_Hierarchy.State.GetNodeVersion(EntityHierarchyNodeId.Root);
            if (!m_StructureChanged && rootVersion == m_RootVersion)
                return;

            m_StructureChanged = false;
            m_RootVersion = rootVersion;

            RecreateRootItems();
            RecreateItemsToExpand();
            RefreshView();
        }

        void RecreateRootItems()
        {
            foreach (var child in m_TreeViewRootItems)
                ((IPoolable)child).ReturnToPool();

            m_TreeViewRootItems.Clear();

            EntityHierarchyPool.ReturnAllVisualElements(this);

            if (m_Hierarchy?.GroupingStrategy == null)
                return;

            using (var rootNodes = m_Hierarchy.State.GetChildren(EntityHierarchyNodeId.Root, Allocator.TempJob))
            {
                foreach (var node in rootNodes)
                    m_TreeViewRootItems.Add(EntityHierarchyPool.GetTreeViewItem(null, node, m_Hierarchy));
            }
        }

        void RecreateItemsToExpand()
        {
            m_TreeViewItemsToExpand.Clear();
            foreach (var treeViewRootItem in m_TreeViewRootItems)
            {
                var hierarchyItem = (EntityHierarchyItem)treeViewRootItem;
                if (hierarchyItem.NodeId.Kind != NodeKind.Scene || m_EntityHierarchyFoldingState.GetFoldingState(hierarchyItem.NodeId) == false)
                    continue;

                m_TreeViewItemsToExpand.Add(hierarchyItem.id);

                if (!hierarchyItem.hasChildren)
                    continue;

                foreach (var childItem in hierarchyItem.Children)
                {
                    if (childItem.NodeId.Kind != NodeKind.SubScene || m_EntityHierarchyFoldingState.GetFoldingState(childItem.NodeId) == false)
                        continue;

                    m_TreeViewItemsToExpand.Add(childItem.id);
                }
            }
        }

        void SwitchViewMode(ViewMode viewMode)
        {
            if (m_ViewMode == viewMode)
                return;

            var previousSelection = default(EntityHierarchyItem);

            if (CurrentView != null)
                m_ViewContainer.Remove(CurrentView);

            switch (viewMode)
            {
                case ViewMode.Full:
                {
                    if (m_ListView.selectedItem is EntityHierarchyItem item)
                        previousSelection = item;

                    m_TreeView.ClearSelection();
                    m_ViewContainer.Add(m_TreeView);
                    break;
                }
                case ViewMode.Search:
                {
                    if (m_TreeView.selectedItem is EntityHierarchyItem item)
                        previousSelection = item;

                    m_ListView.ClearSelection();
                    m_ViewContainer.Add(m_ListView);
                    break;
                }
                default:
                    throw new ArgumentException($"Cannot switch view mode to: {viewMode}");
            }

            m_ViewMode = viewMode;

            RefreshView();

            if (previousSelection != default)
                Select(previousSelection.id);
        }

        void RefreshView()
        {
            switch (m_ViewMode)
            {
                case ViewMode.Full:
                {
                    m_TreeView.PrepareItemsToExpand(m_TreeViewItemsToExpand);
                    m_TreeView.Refresh();
                    break;
                }
                case ViewMode.Search:
                {
                    RefreshSearchView();
                    break;
                }
            }
        }

        void RefreshSearchView()
        {
            if (m_SearcherCacheNeedsRebuild)
            {
                m_ItemsCache.Rebuild(m_TreeViewRootItems.OfType<EntityHierarchyItem>());
                m_SearcherCacheNeedsRebuild = false;
            }

            m_ListViewFilteredItems.Clear();
            var filteredData = m_CurrentQuery?.Apply(m_ItemsCache.Items) ?? m_ItemsCache.Items;
            EntityHierarchyItem lastSubsceneItem = null;
            foreach (var item in filteredData)
            {
                if (item.NodeId.Kind != NodeKind.Entity)
                    continue;

                if (item.parent != null && IsParentedBySubScene(item, out var closestSubScene) && closestSubScene != lastSubsceneItem)
                {
                    lastSubsceneItem = closestSubScene;
                    m_ListViewFilteredItems.Add(lastSubsceneItem);
                }

                m_ListViewFilteredItems.Add(item);
            }

            if (m_ListViewFilteredItems.Count == 0 && m_QueryBuilderResult.IsValid)
            {
                m_SearchEmptyMessage.Title = NoEntitiesFoundTitle;
                m_SearchEmptyMessage.Message = string.Empty;
            }

            if (!m_QueryBuilderResult.IsValid)
            {
                m_SearchEmptyMessage.Title = ComponentTypeNotFoundTitle;
                m_SearchEmptyMessage.Message = string.Format(ComponentTypeNotFoundContent, m_QueryBuilderResult.ErrorComponentType);
            }

            m_ViewContainer.ToggleVisibility(m_ListViewFilteredItems.Count > 0 && m_QueryBuilderResult.IsValid);
            m_SearchEmptyMessage.ToggleVisibility(m_ListViewFilteredItems.Count == 0 || !m_QueryBuilderResult.IsValid);

            m_ListView.Refresh();
        }

        static bool IsParentedBySubScene(EntityHierarchyItem item, out EntityHierarchyItem subSceneItem)
        {
            subSceneItem = null;

            var current = item;
            while (true)
            {
                if (current.parent == null)
                    return false;

                var parent = (EntityHierarchyItem) current.parent;
                switch (parent.NodeId.Kind)
                {
                    case NodeKind.Root:
                    case NodeKind.Scene:
                        return false;
                    case  NodeKind.Entity:
                        current = parent;
                        continue;
                    case NodeKind.SubScene:
                        subSceneItem = parent;
                        return true;
                    default:
                        throw new NotSupportedException($"{nameof(parent.NodeId.Kind)} is not supported in this context");
                }
            }
        }

        void OnLocalSelectionChanged(IEnumerable<object> selection)
        {
            if (selection.FirstOrDefault() is EntityHierarchyItem selectedItem)
                OnLocalSelectionChanged(selectedItem);
        }

        void OnLocalSelectionChanged(EntityHierarchyItem selectedItem)
        {
            if (selectedItem.NodeId.Kind == NodeKind.Entity)
            {
                var entity = selectedItem.NodeId.ToEntity();
                if (entity != Entity.Null)
                {
                    var undoGroup = Undo.GetCurrentGroup();
                    EntitySelectionProxy.SelectEntity(m_Hierarchy.World, entity);

                    // Collapsing the selection of the entity into the selection of the ListView / TreeView item
                    Undo.CollapseUndoOperations(undoGroup);
                }
            }
            else
            {
                // TODO: Deal with non-Entity selections
            }
        }

        void OnGlobalSelectionChanged()
        {
            if (Selection.activeObject is EntitySelectionProxy selectedProxy && selectedProxy.World == m_Hierarchy.World)
            {
                var nodeId = EntityHierarchyNodeId.FromEntity(selectedProxy.Entity);
                if (m_Hierarchy.State.Exists(nodeId))
                    Select(nodeId.GetHashCode());
                else
                    Deselect();
            }
            else
            {
                Deselect();
            }
        }

        VisualElement MakeTreeViewItem() => EntityHierarchyPool.GetVisualElement(this);

        VisualElement MakeListViewItem()
        {
            // ListView changes user created VisualElements in a way that no reversible using public API
            // Wrapping pooled item in a non reusable container prevent us from reusing a pooled item in an eventual checked pseudo state
            var wrapper = new VisualElement();
            wrapper.Add(EntityHierarchyPool.GetVisualElement(this));
            return wrapper;
        }

        void BindTreeViewItem(VisualElement element, ITreeViewItem item) => ((EntityHierarchyItemView)element).SetSource((EntityHierarchyItem)item);

        void BindListViewItem(VisualElement element, int itemIndex) => BindTreeViewItem(element[0], (ITreeViewItem)m_ListView.itemsSource[itemIndex]);
    }
}
