using Unity.Properties.UI;
using Unity.Serialization.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Entities.Editor
{
    class SystemScheduleWindow : DOTSEditorWindow
    {
        static readonly string k_WindowName = L10n.Tr("Systems");
        static readonly string k_ShowFullPlayerLoopString = L10n.Tr("Show Full Player Loop");
        static readonly Vector2 k_MinWindowSize = new Vector2(200, 200);

        VisualElement m_Root;
        CenteredMessageElement m_NoWorld;
        internal SystemScheduleTreeView m_SystemTreeView;
        VisualElement m_WorldSelector;
        VisualElement m_EmptySelectorWhenShowingFullPlayerLoop;

        /// <summary>
        /// Helper container to store session state data.
        /// </summary>
        class State
        {
            /// <summary>
            /// This field controls the showing of full player loop state.
            /// </summary>
            public bool ShowFullPlayerLoop;
        }

        /// <summary>
        /// State data for <see cref="SystemScheduleWindow"/>. This data is persisted between domain reloads.
        /// </summary>
        State m_State;

        [MenuItem(Constants.MenuItems.SystemScheduleWindow, false, Constants.MenuItems.WindowPriority)]
        static void OpenWindow()
        {
            var window = GetWindow<SystemScheduleWindow>();
            window.Show();
        }

        /// <summary>
        /// Build the GUI for the system window.
        /// </summary>
        void OnEnable()
        {
            titleContent = EditorGUIUtility.TrTextContent(k_WindowName, EditorIcons.System);
            minSize = k_MinWindowSize;

            m_Root = new VisualElement { style = { flexGrow = 1 } };
            rootVisualElement.Add(m_Root);

            m_NoWorld = new CenteredMessageElement() { Message = NoWorldMessageContent };
            rootVisualElement.Add(m_NoWorld);
            m_NoWorld.Hide();

            m_State = SessionState<State>.GetOrCreate($"{typeof(SystemScheduleWindow).FullName}+{nameof(State)}+{EditorWindowInstanceKey}");

            Resources.Templates.SystemSchedule.AddStyles(m_Root);
            Resources.Templates.DotsEditorCommon.AddStyles(m_Root);

            CreateToolBar(m_Root);
            CreateTreeViewHeader(m_Root);
            CreateTreeView(m_Root);

            PlayerLoopSystemGraph.Register();

            if (World.All.Count > 0)
                BuildAll();

            PlayerLoopSystemGraph.OnGraphChanged += BuildAll;
            SystemDetailsVisualElement.OnAddFilter += OnAddFilter;
            SystemDetailsVisualElement.OnRemoveFilter += OnRemoveFilter;
        }

        protected override void OnWorldsChanged(bool containsAnyWorld)
        {
            m_Root.ToggleVisibility(containsAnyWorld);
            m_NoWorld.ToggleVisibility(!containsAnyWorld);
        }

        void OnDisable()
        {
            PlayerLoopSystemGraph.OnGraphChanged -= BuildAll;
            SystemDetailsVisualElement.OnAddFilter -= OnAddFilter;
            SystemDetailsVisualElement.OnRemoveFilter -= OnRemoveFilter;

            PlayerLoopSystemGraph.Unregister();
        }

        void CreateToolBar(VisualElement root)
        {
            var toolbar = new Toolbar();
            toolbar.AddToClassList(UssClasses.SystemScheduleWindow.ToolbarContainer);
            root.Add(toolbar);

            m_WorldSelector = CreateWorldSelector();
            toolbar.Add(m_WorldSelector);
            m_EmptySelectorWhenShowingFullPlayerLoop = new ToolbarMenu { text = k_ShowFullPlayerLoopString };
            toolbar.Add(m_EmptySelectorWhenShowingFullPlayerLoop);

            var rightSideContainer = new VisualElement();
            rightSideContainer.AddToClassList(UssClasses.SystemScheduleWindow.ToolbarRightSideContainer);

            AddSearchIcon(rightSideContainer, UssClasses.DotsEditorCommon.SearchIcon);

            var searchElement = AddSearchElement(root, UssClasses.DotsEditorCommon.SearchFieldContainer);
            searchElement.RegisterSearchQueryHandler<int>(query =>
            {
                m_SystemTreeView.SearchFilter = SystemScheduleSearchBuilder.ParseSearchString(SearchFilter);
                BuildAll();
            });

            var dropdownSettings = CreateDropdownSettings(UssClasses.DotsEditorCommon.SettingsIcon);
            dropdownSettings.menu.AppendAction(k_ShowFullPlayerLoopString, a =>
            {
                m_State.ShowFullPlayerLoop = !m_State.ShowFullPlayerLoop;

                UpdateWorldSelectorDisplay();

                if (World.All.Count > 0)
                    BuildAll();
            }, a => m_State.ShowFullPlayerLoop ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
            UpdateWorldSelectorDisplay();
            rightSideContainer.Add(dropdownSettings);
            toolbar.Add(rightSideContainer);
        }

        void UpdateWorldSelectorDisplay()
        {
            m_WorldSelector.ToggleVisibility(!m_State.ShowFullPlayerLoop);
            m_EmptySelectorWhenShowingFullPlayerLoop.ToggleVisibility(m_State.ShowFullPlayerLoop);
        }

        // Manually create header for the tree view.
        void CreateTreeViewHeader(VisualElement root)
        {
            var systemTreeViewHeader = new Toolbar();
            systemTreeViewHeader.AddToClassList(UssClasses.SystemScheduleWindow.TreeView.Header);

            var systemHeaderLabel = new Label("Systems");
            systemHeaderLabel.AddToClassList(UssClasses.SystemScheduleWindow.TreeView.System);

            var entityHeaderLabel = new Label("Matches")
            {
                tooltip = "The number of entities that match the queries at the end of the frame."
            };
            entityHeaderLabel.AddToClassList(UssClasses.SystemScheduleWindow.TreeView.Matches);

            var timeHeaderLabel = new Label("Time (ms)")
            {
                tooltip = "Average running time."
            };
            timeHeaderLabel.AddToClassList(UssClasses.SystemScheduleWindow.TreeView.Time);

            systemTreeViewHeader.Add(systemHeaderLabel);
            systemTreeViewHeader.Add(entityHeaderLabel);
            systemTreeViewHeader.Add(timeHeaderLabel);

            root.Add(systemTreeViewHeader);
        }

        void CreateTreeView(VisualElement root)
        {
            m_SystemTreeView = new SystemScheduleTreeView(EditorWindowInstanceKey)
            {
                style = { flexGrow = 1 },
                SearchFilter = SystemScheduleSearchBuilder.ParseSearchString(SearchFilter)
            };
            root.Add(m_SystemTreeView);
        }

        void BuildAll()
        {
            m_SystemTreeView.Refresh(m_State.ShowFullPlayerLoop ? null : SelectedWorld);
        }

        protected override void OnUpdate() { }

        protected override void OnWorldSelected(World world)
        {
            if (m_State.ShowFullPlayerLoop)
                return;

            BuildAll();
        }

        void OnAddFilter(string toAdd)
        {
            AddStringToSearchField(toAdd);
        }

        void OnRemoveFilter(string toRemove)
        {
            RemoveStringFromSearchField(toRemove);
        }
    }
}
