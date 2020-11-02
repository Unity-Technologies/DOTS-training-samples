using System;
using System.Linq;
using Unity.Assertions;
using Unity.Properties.UI;
using Unity.Serialization.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    abstract class DOTSEditorWindow : EditorWindow
    {
        readonly WorldsChangeDetector m_WorldsChangeDetector = new WorldsChangeDetector();

        ToolbarMenu m_WorldSelector;
        VisualElement m_SearchFieldContainer;
        SearchElement m_SearchField;
        Image m_SearchIcon;
        bool m_PreviousShowAdvancedWorldsValue;
        World m_SelectedWorld;
        bool m_SelectedWorldChanged;

        [SerializeField]
        string m_EditorWindowInstanceKey;

        protected static string NoWorldMessageContent { get; } = L10n.Tr("No World available");

        protected string EditorWindowInstanceKey
        {
            get
            {
                if (string.IsNullOrEmpty(m_EditorWindowInstanceKey))
                    m_EditorWindowInstanceKey = Guid.NewGuid().ToString("N");

                return m_EditorWindowInstanceKey;
            }
        }

        internal BaseStateContainer BaseState => SessionState<BaseStateContainer>.GetOrCreate($"{GetType().Name}.{nameof(BaseStateContainer)}+{EditorWindowInstanceKey}");

        protected string SearchFilter
        {
            get => IsSearchFieldVisible ? BaseState.SearchFilter : null;
            private set
            {
                BaseState.SearchFilter = value;
                m_SearchField.value = value;
            }
        }

        protected internal World SelectedWorld
        {
            get => m_SelectedWorld;
            set
            {
                if (m_SelectedWorld == value)
                    return;

                m_SelectedWorld = value;
                m_WorldSelector.text = value?.Name ?? string.Empty;
                m_SelectedWorldChanged = true;
            }
        }

        // Internal for unit tests
        internal void ChangeCurrentWorld(World newWorld)
        {
            BaseState.SelectedWorldName = newWorld.Name;
            SelectedWorld = newWorld;
            Update();
        }

        protected bool IsSearchFieldVisible => m_SearchField != null && UIElementHelper.IsVisible(m_SearchField);

        World FindSelectedWorld()
        {
            if (World.All.Count == 0)
                return null;

            var selectedWorld = World.All[0];

            if (string.IsNullOrWhiteSpace(BaseState.SelectedWorldName))
                return selectedWorld;

            foreach (var world in World.All)
            {
                if (world.Name == BaseState.SelectedWorldName)
                    return world;
            }

            return selectedWorld;
        }

        protected ToolbarMenu CreateWorldSelector()
        {
            m_WorldSelector = new ToolbarMenu
            {
                name = "worldMenu",
                variant = ToolbarMenu.Variant.Popup
            };

            UpdateWorldDropDownMenu();
            SelectedWorld = FindSelectedWorld();

            m_PreviousShowAdvancedWorldsValue = UserSettings<AdvancedSettings>.GetOrCreate(Constants.Settings.Advanced).ShowAdvancedWorlds;

            return m_WorldSelector;
        }

        protected SearchElement AddSearchElement(VisualElement parent, string ussClass)
        {
            m_SearchFieldContainer = new VisualElement();
            m_SearchFieldContainer.AddToClassList(ussClass);

            m_SearchField = new SearchElement();
            m_SearchField.SetValueWithoutNotify(string.IsNullOrEmpty(BaseState.SearchFilter) ? string.Empty : BaseState.SearchFilter);
            m_SearchField.AddToClassList(UssClasses.DotsEditorCommon.SearchField);
            m_SearchField.RegisterSearchQueryHandler<int>(query =>
            {
                if (BaseState.IsSearchFieldVisible)
                    BaseState.SearchFilter = query.SearchString;
            });

            UIElementHelper.ToggleVisibility(m_SearchField, BaseState.IsSearchFieldVisible);

            m_SearchFieldContainer.Add(m_SearchField);

            parent.Add(m_SearchFieldContainer);

            return m_SearchField;
        }

        protected void SetSearchFieldVisibility(bool visible)
        {
            BaseState.IsSearchFieldVisible = visible;
            if (m_SearchField == null)
                return;

            if (visible)
            {
                UIElementHelper.Show(m_SearchField);
                m_SearchField.Q("unity-text-input").Focus();
            }
            else
            {
                UIElementHelper.Hide(m_SearchField);
            }

            m_SearchField.Search(SearchFilter);
        }

        protected void AddSearchIcon(VisualElement parent, string ussClass)
        {
            var searchIconContainer = new VisualElement();
            searchIconContainer.AddToClassList(UssClasses.DotsEditorCommon.SearchIconContainer);

            m_SearchIcon = new Image();
            Resources.Templates.DotsEditorCommon.AddStyles(m_SearchIcon);
            m_SearchIcon.AddToClassList(UssClasses.DotsEditorCommon.CommonResources);
            m_SearchIcon.AddToClassList(ussClass);

            m_SearchIcon.RegisterCallback<MouseUpEvent>(evt => SetSearchFieldVisibility(!IsSearchFieldVisible));

            searchIconContainer.Add(m_SearchIcon);
            parent.Add(searchIconContainer);
        }

        protected ToolbarMenu CreateDropdownSettings(string ussClass)
        {
            var dropdownSettings = new ToolbarMenu()
            {
                name = "dropdownSettings",
                variant = ToolbarMenu.Variant.Popup
            };

            Resources.Templates.DotsEditorCommon.AddStyles(dropdownSettings);
            dropdownSettings.AddToClassList(UssClasses.DotsEditorCommon.CommonResources);
            dropdownSettings.AddToClassList(ussClass);

            var arrow = dropdownSettings.Q(className: "unity-toolbar-menu__arrow");
            arrow.style.backgroundImage = null;

            return dropdownSettings;
        }

        public void Update()
        {
            if (NeedToChangeWorldDropDownMenu())
            {
                SelectedWorld = FindSelectedWorld();
                UpdateWorldDropDownMenu();
            }

            if (m_SelectedWorldChanged)
            {
                m_SelectedWorldChanged = false;
                OnWorldSelected(m_SelectedWorld);
            }

            OnUpdate();
        }

        bool NeedToChangeWorldDropDownMenu()
        {
            if (null == m_WorldSelector)
                return false;

            if (m_WorldsChangeDetector.WorldsChanged())
                return true;

            var showAdvancedWorlds = UserSettings<AdvancedSettings>.GetOrCreate(Constants.Settings.Advanced).ShowAdvancedWorlds;
            if (m_PreviousShowAdvancedWorldsValue != showAdvancedWorlds)
            {
                m_PreviousShowAdvancedWorldsValue = showAdvancedWorlds;
                return true;
            }

            return false;
        }

        protected void UpdateWorldDropDownMenu()
        {
            Assert.IsNotNull(m_WorldSelector);

            var menu = m_WorldSelector.menu;
            var menuItemsCount = menu.MenuItems().Count;

            for (var i = 0; i < menuItemsCount; i++)
            {
                menu.RemoveItemAt(0);
            }

            var advancedSettings = UserSettings<AdvancedSettings>.GetOrCreate(Constants.Settings.Advanced);

            if (World.All.Count > 0)
                AppendWorldMenu(menu, advancedSettings.ShowAdvancedWorlds);

            OnWorldsChanged(World.All.Count > 0);
        }

        protected virtual void OnWorldsChanged(bool containsAnyWorld) { }

        void AppendWorldMenu(DropdownMenu menu, bool showAdvancedWorlds)
        {
            var worldCategories = WorldCategoryHelper.Categories;

            foreach (var category in worldCategories)
            {
                if (showAdvancedWorlds)
                {
                    menu.AppendAction(category.Name.ToUpper(), null, DropdownMenuAction.Status.Disabled);
                    AppendWorlds(menu, category);
                    menu.AppendSeparator();
                }
                else if (category.Flag == WorldFlags.Live)
                {
                    AppendWorlds(menu, category);
                    break;
                }
            }
        }

        void AppendWorlds(DropdownMenu menu, WorldCategoryHelper.Category category)
        {
            foreach (var world in category.Worlds)
            {
                menu.AppendAction(world.Name, OnWorldSelected, a =>
                    (SelectedWorld == world)
                    ? DropdownMenuAction.Status.Checked
                    : DropdownMenuAction.Status.Normal, world);
            }
        }

        void OnWorldSelected(DropdownMenuAction action)
        {
            var world = action.userData as World;
            BaseState.SelectedWorldName = world?.Name;
            SelectedWorld = world;
        }

        protected void AddStringToSearchField(string toAdd)
        {
            if (SearchUtility.SplitSearchStringBySpace(SearchFilter).Any(singleString => singleString.Equals(toAdd, StringComparison.OrdinalIgnoreCase)))
                return;

            SearchFilter += string.IsNullOrWhiteSpace(SearchFilter)
                ? toAdd + " "
                : " " + toAdd + " ";

            SetSearchFieldVisibility(true);
        }

        protected void RemoveStringFromSearchField(string toRemove)
        {
            if (string.IsNullOrWhiteSpace(SearchFilter)
                || SearchFilter.IndexOf(toRemove, StringComparison.OrdinalIgnoreCase) == -1)
                return;

            var tempResult = string.Empty;
            foreach (var singleString in SearchUtility.SplitSearchStringBySpace(SearchFilter))
            {
                if (singleString.Equals(toRemove, StringComparison.OrdinalIgnoreCase))
                    continue;

                tempResult += singleString + " ";
            }

            SearchFilter = tempResult.Trim();

            SetSearchFieldVisibility(true);
        }

        protected abstract void OnUpdate();
        protected abstract void OnWorldSelected(World world);

        internal class BaseStateContainer
        {
            public string SelectedWorldName;
            public string SearchFilter;
            public bool IsSearchFieldVisible = true; // Visible by default
        }
    }
}
