using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Editor.Bridge;
using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    class SystemScheduleTreeView : VisualElement
    {
        static readonly string k_NoSystemsFoundTitle = L10n.Tr("No system matches your search");
        static readonly string k_ComponentTypeNotFoundTitle = L10n.Tr("Type not found");
        static readonly string k_ComponentTypeNotFoundContent = L10n.Tr("\"{0}\" is not a component type");

        readonly TreeView m_SystemTreeView;
        internal readonly IList<ITreeViewItem> m_TreeRootItems = new List<ITreeViewItem>();
        SystemDetailsVisualElement m_SystemDetailsVisualElement;
        SystemTreeViewItem m_LastSelectedItem;
        int m_LastSelectedItemId;
        World m_World;
        List<Type> m_SystemDependencyList = new List<Type>();
        CenteredMessageElement m_SearchEmptyMessage;
        int m_ScrollToItemId = -1;

        public SystemScheduleSearchBuilder.ParseResult SearchFilter { get; set; }

        /// <summary>
        /// Constructor of the tree view.
        /// </summary>
        public SystemScheduleTreeView(string editorWindowInstanceKey)
        {
            m_SystemTreeView = new TreeView(m_TreeRootItems, Constants.ListView.ItemHeight, MakeItem, BindItem)
            {
                viewDataKey = $"{Constants.State.ViewDataKeyPrefix}{typeof(SystemScheduleWindow).FullName}+{editorWindowInstanceKey}",
                style = { flexGrow = 1 }
            };

            m_SystemTreeView.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (m_ScrollToItemId == -1)
                    return;

                var tempId = m_ScrollToItemId;
                m_ScrollToItemId = -1;
                
                if (m_SystemTreeView.FindItem(tempId) != null)
                    m_SystemTreeView.ScrollToItem(tempId);
            });

            Add(m_SystemTreeView);
            CreateSystemDetailsSection();
            m_SearchEmptyMessage = new CenteredMessageElement { Title = k_NoSystemsFoundTitle};
            m_SearchEmptyMessage.Hide();
            Add(m_SearchEmptyMessage);
        }

        void CreateSystemDetailsSection()
        {
            m_SystemDetailsVisualElement = new SystemDetailsVisualElement();
            m_SystemTreeView.onSelectionChange += (selectedItems) =>
            {
                var item = selectedItems.OfType<SystemTreeViewItem>().FirstOrDefault();
                if (null == item)
                    return;

                if (!item.selectedSystem.Valid)
                {
                    if (Contains(m_SystemDetailsVisualElement))
                        Remove(m_SystemDetailsVisualElement);

                    return;
                }

                // Remember last selected item id so that query information can be properly updated.
                m_LastSelectedItemId = item.id;
                m_LastSelectedItem = item;
                m_ScrollToItemId = item.id;

                // Start fresh.
                if (Contains(m_SystemDetailsVisualElement))
                    Remove(m_SystemDetailsVisualElement);

                m_SystemDetailsVisualElement.Target = item;
                m_SystemDetailsVisualElement.SearchFilter = SearchFilter;
                m_SystemDetailsVisualElement.Parent = this;
                m_SystemDetailsVisualElement.LastSelectedItem = m_LastSelectedItem;
                Add(m_SystemDetailsVisualElement);
            };
        }

        VisualElement MakeItem()
        {
            var systemItem = SystemSchedulePool.GetSystemInformationVisualElement(this);
            systemItem.World = m_World;
            return systemItem;
        }

        public void Refresh(World world)
        {
            if (m_World != world && Contains(m_SystemDetailsVisualElement))
                Remove(m_SystemDetailsVisualElement);

            m_World = world;

            foreach (var root in m_TreeRootItems.OfType<SystemTreeViewItem>())
            {
                root.ReturnToPool();
            }
            m_TreeRootItems.Clear();

            if (World.All.Count > 0 && string.IsNullOrEmpty(SearchFilter.ErrorComponentType) && string.IsNullOrEmpty(SearchFilter.ErrorSdSystemName))
            {
                var graph = PlayerLoopSystemGraph.Current;

                if (!SearchFilter.IsEmpty && SearchFilter.DependencySystemNames.Any() && SearchFilter.DependencySystemTypes.Any())
                {
                    SystemScheduleUtilities.GetSystemDepListFromSystemTypes(SearchFilter.DependencySystemTypes, m_SystemDependencyList);
                    if (null == m_SystemDependencyList || !m_SystemDependencyList.Any())
                    {
                        if (this.Contains(m_SystemDetailsVisualElement))
                            Remove(m_SystemDetailsVisualElement);
                    }
                }

                foreach (var node in graph.Roots)
                {
                    if (!node.ShowForWorld(m_World))
                        continue;

                    var item = SystemSchedulePool.GetSystemTreeViewItem(graph, node, null, m_World);

                    PopulateAllChildren(item);
                    m_TreeRootItems.Add(item);
                }
            }

            Refresh();
        }

        void PopulateAllChildren(SystemTreeViewItem item)
        {
            if (item.id == m_LastSelectedItemId)
            {
                m_LastSelectedItem = item;
                m_SystemDetailsVisualElement.LastSelectedItem = m_LastSelectedItem;
            }

            if (!item.HasChildren)
                return;

            item.PopulateChildren(SearchFilter, m_SystemDependencyList);

            foreach (var child in item.children)
            {
                PopulateAllChildren(child as SystemTreeViewItem);
            }
        }

        /// <summary>
        /// Refresh tree view to update with latest information.
        /// </summary>
        void Refresh()
        {
            // This is needed because `ListView.Refresh` will re-create all the elements.
            SystemSchedulePool.ReturnAllToPool(this);
            m_SystemTreeView.Refresh();

            // System details need to be updated also.
            m_SystemDetailsVisualElement.Target = m_LastSelectedItem;
            m_SystemDetailsVisualElement.SearchFilter = SearchFilter;

            // Check if there is search result
            if (!SearchFilter.IsEmpty)
            {
                var hasSearchResult = m_TreeRootItems.Any(item => item.children.Any());
                m_SearchEmptyMessage.ToggleVisibility(!hasSearchResult);
                m_SystemTreeView.ToggleVisibility(hasSearchResult);
                m_SystemDetailsVisualElement.ToggleVisibility(hasSearchResult);

                if (string.IsNullOrEmpty(SearchFilter.ErrorComponentType))
                {
                    m_SearchEmptyMessage.Title = k_NoSystemsFoundTitle;
                    m_SearchEmptyMessage.Message = string.Empty;
                }
                else
                {
                    m_SearchEmptyMessage.Title = k_ComponentTypeNotFoundTitle;
                    m_SearchEmptyMessage.Message = string.Format(k_ComponentTypeNotFoundContent, SearchFilter.ErrorComponentType);
                }
            }
            else
            {
                m_SearchEmptyMessage.Hide();
                m_SystemTreeView.Show();
                m_SystemDetailsVisualElement.Show();
            }
        }

        void BindItem(VisualElement element, ITreeViewItem item)
        {
            var target = item as SystemTreeViewItem;
            var systemInformationElement = element as SystemInformationVisualElement;
            if (null == systemInformationElement)
                return;

            systemInformationElement.Target = target;
            systemInformationElement.World = m_World;
            systemInformationElement.Update();
        }
    }
}
