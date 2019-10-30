using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine.Assertions;
using System.Linq;

namespace UnityEditor.Performance.ProfileAnalyzer
{
    class ThreadTreeViewItem : TreeViewItem
    {
        public readonly ThreadIdentifier threadIdentifier;

        public ThreadTreeViewItem(int id, int depth, string displayName, ThreadIdentifier threadIdentifier) : base(id, depth, displayName)
        {
            this.threadIdentifier = threadIdentifier;
        }
    }

    class ThreadTable : TreeView
    {
        const float kRowHeights = 20f;
        readonly List<TreeViewItem> m_Rows = new List<TreeViewItem>(100);

        List<string> m_ThreadNames;
        List<string> m_ThreadUINames;
        ThreadIdentifier m_AllThreadIdentifier;
        ThreadSelection m_ThreadSelection;

        private GUIStyle activeLineStyle;

        // All columns
        public enum MyColumns
        {
            ThreadName,
            State,
            GroupName
        }

        public enum SortOption
        {
            ThreadName,
            GroupName
        }

        // Sort options per column
        SortOption[] m_SortOptions =
        {
            SortOption.ThreadName,
            SortOption.ThreadName,
            SortOption.GroupName
        };

        public ThreadTable(TreeViewState state, MultiColumnHeader multicolumnHeader, List<string> threadNames, List<string> threadUINames, ThreadSelection threadSelection) : base(state, multicolumnHeader)
        {
            m_AllThreadIdentifier = new ThreadIdentifier();
            m_AllThreadIdentifier.SetName("All");
            m_AllThreadIdentifier.SetAll();

            Assert.AreEqual(m_SortOptions.Length, Enum.GetValues(typeof(MyColumns)).Length, "Ensure number of sort options are in sync with number of MyColumns enum values");

            // Custom setup
            rowHeight = kRowHeights;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            // extraSpaceBeforeIconAndLabel = 0;
            multicolumnHeader.sortingChanged += OnSortingChanged;

            m_ThreadNames = threadNames; 
            m_ThreadUINames = threadUINames;
            m_ThreadSelection = new ThreadSelection(threadSelection);
            Reload();
        }

        public void ClearThreadSelection()
        {
            m_ThreadSelection.selection.Clear();
            m_ThreadSelection.groups.Clear();
            Reload();
        }

        public ThreadSelection GetThreadSelection()
        {
            return m_ThreadSelection;
        }

        protected int GetChildCount(ThreadIdentifier selectedThreadIdentifier, out int selected)
        {
            int count = 0;
            int selectedCount = 0;

            if (selectedThreadIdentifier.index == ThreadIdentifier.kAll)
            {
                if (selectedThreadIdentifier.name == "All")
                {
                    for (int index = 0; index < m_ThreadNames.Count; ++index)
                    {
                        var threadNameWithIndex = m_ThreadNames[index];
                        var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);

                        if (threadIdentifier.index != ThreadIdentifier.kAll)
                        {
                            count++;
                            if (m_ThreadSelection.selection.Contains(threadNameWithIndex))
                                selectedCount++;
                        }
                    }
                }
                else
                {
                    for (int index = 0; index < m_ThreadNames.Count; ++index)
                    {
                        var threadNameWithIndex = m_ThreadNames[index];
                        var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);

                        if (selectedThreadIdentifier.name == threadIdentifier.name &&
                            threadIdentifier.index != ThreadIdentifier.kAll)
                        {
                            count++;
                            if (m_ThreadSelection.selection.Contains(threadNameWithIndex))
                                selectedCount++;
                        }
                    }
                }
            }

            selected = selectedCount;
            return count;
        }

        protected override TreeViewItem BuildRoot()
        {
            int idForHiddenRoot = -1;
            int depthForHiddenRoot = -1;
            ProfileTreeViewItem root = new ProfileTreeViewItem(idForHiddenRoot, depthForHiddenRoot, "root", null);

            int depth = 0;

            var top = new ThreadTreeViewItem(-1, depth, m_AllThreadIdentifier.name, m_AllThreadIdentifier);
            root.AddChild(top);

            var expandList = new List<int>() {-1};
            string lastThreadName = "";
            TreeViewItem node = root;
            for (int index = 0; index < m_ThreadNames.Count; ++index)
            {
                var threadNameWithIndex = m_ThreadNames[index];
                if (threadNameWithIndex == m_AllThreadIdentifier.threadNameWithIndex)
                    continue;

                var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);
                var item = new ThreadTreeViewItem(index, depth, m_ThreadUINames[index], threadIdentifier);

                if (threadIdentifier.name != lastThreadName)
                {
                    // New threads at root
                    node = top;
                    depth = 0;
                }

                node.AddChild(item);
                

                if (threadIdentifier.name != lastThreadName)
                {
                    // Extra instances hang of the parent
                    lastThreadName = threadIdentifier.name;
                    node = item;
                    depth = 1;
                }
            }
            
            SetExpanded(expandList);
            
            SetupDepthsFromParentsAndChildren(root);

            return root;
        }
        
        private void BuildRowRecursive(IList<TreeViewItem> rows, TreeViewItem item)
        {
            //if (item.children == null)
            //    return;

            if (!IsExpanded(item.id))
                return;

            foreach (ThreadTreeViewItem subNode in item.children)
            {
                rows.Add(subNode);

                if (subNode.children!=null)
                    BuildRowRecursive(rows, subNode);
            }
        }

        private void BuildAllRows(IList<TreeViewItem> rows, TreeViewItem rootItem)
        {
            rows.Clear();
            if (rootItem == null)
                return;

            if (rootItem.children == null)
                return;

            foreach (ThreadTreeViewItem node in rootItem.children)
            {
                rows.Add(node);

                if (node.children != null)
                    BuildRowRecursive(rows, node);
            }
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            BuildAllRows(m_Rows, root);

            SortIfNeeded(m_Rows);

            return m_Rows;
        }

        void OnSortingChanged(MultiColumnHeader _multiColumnHeader)
        {
            SortIfNeeded(GetRows());
        }

        void SortIfNeeded(IList<TreeViewItem> rows)
        {
            if (rows.Count <= 1)
            {
                return;
            }

            if (multiColumnHeader.sortedColumnIndex == -1)
            {
                return; // No column to sort for (just use the order the data are in)
            }

            // Sort the roots of the existing tree items
            SortByMultipleColumns();
            
            BuildAllRows(rows, rootItem);

            Repaint();
        }

        string GetItemGroupName(ThreadTreeViewItem item)
        {
            string groupName;
            string threadName = item.threadIdentifier.name;
            threadName = ProfileData.GetThreadNameWithoutGroup(item.threadIdentifier.name, out groupName);

            return groupName;
        }

        void SortByMultipleColumns()
        {
            int[] sortedColumns = multiColumnHeader.state.sortedColumns;

            if (sortedColumns.Length == 0)
            {
                return;
            }

            var myTypes = rootItem.children.Cast<ThreadTreeViewItem>();
            var orderedQuery = InitialOrder(myTypes, sortedColumns);
            for (int i = 1; i < sortedColumns.Length; i++)
            {
                SortOption sortOption = m_SortOptions[sortedColumns[i]];
                bool ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);

                switch (sortOption)
                {
                    case SortOption.GroupName:
                        orderedQuery = orderedQuery.ThenBy(l => GetItemGroupName(l), ascending);
                        break;
                    case SortOption.ThreadName:
                        orderedQuery = orderedQuery.ThenBy(l => l.displayName, ascending);
                        break;
                }
            }

            rootItem.children = orderedQuery.Cast<TreeViewItem>().ToList();
        }

        IOrderedEnumerable<ThreadTreeViewItem> InitialOrder(IEnumerable<ThreadTreeViewItem> myTypes, int[] history)
        {
            SortOption sortOption = m_SortOptions[history[0]];
            bool ascending = multiColumnHeader.IsSortedAscending(history[0]);
            switch (sortOption)
            {
                case SortOption.GroupName:
                    return myTypes.Order(l => GetItemGroupName(l), ascending);
                case SortOption.ThreadName:
                    return myTypes.Order(l => l.displayName, ascending);
                default:
                    Assert.IsTrue(false, "Unhandled enum");
                    break;
            }

            // default
            return myTypes.Order(l => l.displayName, ascending);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            ThreadTreeViewItem item = (ThreadTreeViewItem)args.item;

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, (MyColumns)args.GetColumn(i), ref args);
            }
        }

        bool ThreadSelected(ThreadIdentifier selectedThreadIdentifier)
        {
            if (ProfileAnalyzer.MatchThreadFilter(selectedThreadIdentifier.threadNameWithIndex, m_ThreadSelection.selection))
                return true;

            // If querying the 'All' filter then check if all selected
            if (selectedThreadIdentifier.threadNameWithIndex == m_AllThreadIdentifier.threadNameWithIndex)
            {
                // Check all threads without All in the name are selected
                foreach (var threadNameWithIndex in m_ThreadNames)
                {
                    var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);
                    if (threadIdentifier.index == ThreadIdentifier.kAll || threadIdentifier.index == ThreadIdentifier.kSingle)
                        continue;

                    if (!m_ThreadSelection.selection.Contains(threadNameWithIndex))
                        return false;
                }

                return true;
            }

            // Need to check 'all' and thread group all.
            if (selectedThreadIdentifier.index == ThreadIdentifier.kAll)
            {
                // Count all threads that match this thread group
                int count = 0;
                foreach (var threadNameWithIndex in m_ThreadNames)
                {
                    var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);
                    if (threadIdentifier.index == ThreadIdentifier.kAll || threadIdentifier.index == ThreadIdentifier.kSingle)
                        continue;
                    
                    if (selectedThreadIdentifier.name != threadIdentifier.name)
                        continue;

                    count++;
                }

                // Count all the threads we have selected that match this thread group
                int selectedCount = 0;
                foreach (var threadNameWithIndex in m_ThreadSelection.selection)
                {
                    var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);
                    if (selectedThreadIdentifier.name != threadIdentifier.name)
                        continue;
                    if (threadIdentifier.index > count)
                        continue;

                    selectedCount++;
                }

                if (count == selectedCount)
                    return true;
            }

            return false;
        }


        void CellGUI(Rect cellRect, ThreadTreeViewItem item, MyColumns column, ref RowGUIArgs args)
        {
            // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
            CenterRectUsingSingleLineHeight(ref cellRect);

            switch (column)
            {
                case MyColumns.ThreadName:
                    {
                        args.rowRect = cellRect;
                        // base.RowGUI(args);    // Required to show tree indenting

                        // Draw manually to keep indenting by add a tooltip
                        Rect rect = cellRect;
                        if (Event.current.rawType == EventType.Repaint)
                        {
                            int selectedChildren;
                            int childCount = GetChildCount(item.threadIdentifier, out selectedChildren);

                            string text;
                            string tooltip;
                            string fullThreadName = item.threadIdentifier.name;
                            string groupName = GetItemGroupName(item);
                            // string threadName = ProfileData.GetThreadNameWithoutGroup(fullThreadName, out groupName);

                            if (childCount <= 1)
                            {
                                text = item.displayName;
                                tooltip = (groupName == "") ? text : string.Format("{0}\n{1}", text, groupName);
                            }
                            else if (selectedChildren != childCount)
                            {
                                text = string.Format("{0} ({1} of {2})", fullThreadName, selectedChildren, childCount);
                                tooltip = (groupName == "") ? text : string.Format("{0}\n{1}", text, groupName);
                            }
                            else
                            {
                                text = string.Format("{0} (All)", fullThreadName);
                                tooltip = (groupName == "") ? text : string.Format("{0}\n{1}", text, groupName);
                            }
                            var content = new GUIContent(text, tooltip);

                            if (activeLineStyle == null)
                            {
                                // activeLineStyle = DefaultStyles.boldLabel;
                                activeLineStyle = new GUIStyle(DefaultStyles.label);
                                activeLineStyle.normal.textColor = DefaultStyles.boldLabel.onActive.textColor;
                            }
                       
                            // The rect is assumed indented and sized after the content when pinging
                            float indent = GetContentIndent(item) + extraSpaceBeforeIconAndLabel;
                            rect.xMin += indent;

                            int iconRectWidth = 16;
                            int kSpaceBetweenIconAndText = 2;

                            // Draw icon
                            Rect iconRect = rect;
                            iconRect.width = iconRectWidth;
                            // iconRect.x += 7f;

                            Texture icon = args.item.icon;
                            if (icon != null)
                                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);

                            rect.xMin += icon == null ? 0 : iconRectWidth + kSpaceBetweenIconAndText;

                            //bool mouseOver = rect.Contains(Event.current.mousePosition);
                            //DefaultStyles.label.Draw(rect, content, mouseOver, false, args.selected, args.focused);

                            // Must use this call to draw tooltip
                            EditorGUI.LabelField(rect, content, args.selected ? activeLineStyle : DefaultStyles.label);
                        }
                    }
                    break;
                case MyColumns.GroupName:
                    {
                        string groupName = GetItemGroupName(item);
                        var content = new GUIContent(groupName, groupName);
                        EditorGUI.LabelField(cellRect, content);
                    }
                    break;
                case MyColumns.State:
                    bool oldState = ThreadSelected(item.threadIdentifier);
                    bool newState = EditorGUI.Toggle(cellRect, oldState);
                    if (newState != oldState)
                    {
                        if (item.threadIdentifier.threadNameWithIndex == m_AllThreadIdentifier.threadNameWithIndex)
                        {
                            // Record active groups
                            m_ThreadSelection.groups.Clear();
                            if (newState)
                            {
                                if (!m_ThreadSelection.groups.Contains(item.threadIdentifier.threadNameWithIndex))
                                    m_ThreadSelection.groups.Add(item.threadIdentifier.threadNameWithIndex);
                            }

                            // Update selection
                            m_ThreadSelection.selection.Clear();
                            if (newState)
                            {
                                foreach (string threadNameWithIndex in m_ThreadNames)
                                {
                                    if (threadNameWithIndex != m_AllThreadIdentifier.threadNameWithIndex)
                                    {
                                        var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);
                                        if (threadIdentifier.index != ThreadIdentifier.kAll)
                                        {
                                            m_ThreadSelection.selection.Add(threadNameWithIndex);
                                        }
                                    }
                                }
                            }
                        }
                        else if (item.threadIdentifier.index == ThreadIdentifier.kAll)
                        {
                            // Record active groups
                            if (newState)
                            {
                                if (!m_ThreadSelection.groups.Contains(item.threadIdentifier.threadNameWithIndex))
                                    m_ThreadSelection.groups.Add(item.threadIdentifier.threadNameWithIndex);
                            }
                            else
                            {
                                m_ThreadSelection.groups.Remove(item.threadIdentifier.threadNameWithIndex);
                                // When turning off a sub group, turn of the 'all' group too
                                m_ThreadSelection.groups.Remove(m_AllThreadIdentifier.threadNameWithIndex);
                            }

                            // Update selection
                            if (newState)
                            {
                                foreach (string threadNameWithIndex in m_ThreadNames)
                                {
                                    var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);
                                    if (threadIdentifier.name == item.threadIdentifier.name &&
                                        threadIdentifier.index != ThreadIdentifier.kAll)
                                    {
                                        if (!m_ThreadSelection.selection.Contains(threadNameWithIndex))
                                            m_ThreadSelection.selection.Add(threadNameWithIndex);
                                    }
                                }
                            }
                            else
                            {
                                var removeSelection = new List<string>();
                                foreach (string threadNameWithIndex in m_ThreadSelection.selection)
                                {
                                    var threadIdentifier = new ThreadIdentifier(threadNameWithIndex);
                                    if (threadIdentifier.name == item.threadIdentifier.name &&
                                        threadIdentifier.index != ThreadIdentifier.kAll)
                                    {
                                        removeSelection.Add(threadNameWithIndex);
                                    }
                                }
                                foreach (string threadNameWithIndex in removeSelection)
                                {
                                    m_ThreadSelection.selection.Remove(threadNameWithIndex);
                                }
                            }
                        }
                        else
                        {
                            if (newState)
                            {
                                m_ThreadSelection.selection.Add(item.threadIdentifier.threadNameWithIndex);
                            }
                            else
                            {
                                m_ThreadSelection.selection.Remove(item.threadIdentifier.threadNameWithIndex);

                                // Turn off any group its in too
                                var groupIdentifier = new ThreadIdentifier(item.threadIdentifier);
                                groupIdentifier.SetAll();
                                m_ThreadSelection.groups.Remove(groupIdentifier.threadNameWithIndex);

                                // Turn of the 'all' group too
                                m_ThreadSelection.groups.Remove(m_AllThreadIdentifier.threadNameWithIndex);
                            }
                        }
                    }
                    break;
            }
        }


        // Misc
        //--------

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        struct HeaderData
        {
            public GUIContent content;
            public float width;
            public float minWidth;
            public bool autoResize;
            public bool allowToggleVisibility;

            public HeaderData(string name, string tooltip = "", float _width = 50, float _minWidth = 30, bool _autoResize = true, bool _allowToggleVisibility = true)
            {
                content = new GUIContent(name, tooltip);
                width = _width;
                minWidth = _minWidth;
                autoResize = _autoResize;
                allowToggleVisibility = _allowToggleVisibility;
            }
        }

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
        {
            var columnList = new List<MultiColumnHeaderState.Column>();
            HeaderData[] headerData = new HeaderData[]
            {
                new HeaderData("Thread", "Thread Name", 350, 100, true, false),
                new HeaderData("Show", "Check to show this thread in the analysis views", 40, 100, false, false),
                new HeaderData("Group", "Thread Group", 100, 100, true, false),

            };
            foreach (var header in headerData)
            {
                columnList.Add(new MultiColumnHeaderState.Column
                {
                    headerContent = header.content,
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = header.width,
                    minWidth = header.minWidth,
                    autoResize = header.autoResize,
                    allowToggleVisibility = header.allowToggleVisibility
                });
            };
            var columns = columnList.ToArray();

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(MyColumns)).Length, "Number of columns should match number of enum values: You probably forgot to update one of them.");

            var state = new MultiColumnHeaderState(columns);
            state.visibleColumns = new int[] {
                        (int)MyColumns.ThreadName,
                        (int)MyColumns.State,
                        //(int)MyColumns.GroupName
                    };
            return state;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);

            if (selectedIds.Count > 0)
            {
            }
        }
    }

    public class ThreadSelectionWindow : EditorWindow
    {
        ProfileAnalyzerWindow m_ProfileAnalyzerWindow;
        TreeViewState m_ThreadTreeViewState;
        MultiColumnHeaderState m_ThreadMulticolumnHeaderState;
        ThreadTable m_ThreadTable;
        
        static public bool IsOpen()
        {
            UnityEngine.Object[] windows = Resources.FindObjectsOfTypeAll(typeof(ThreadSelectionWindow));
            if (windows != null && windows.Length > 0)
                return true;

            return false;
        }

        static public ThreadSelectionWindow Open(float screenX, float screenY, ProfileAnalyzerWindow profileAnalyzerWindow, ThreadSelection threadSelection, List<string> threadNames, List<string> threadUINames)
        {
            ThreadSelectionWindow window = GetWindow<ThreadSelectionWindow>("Threads");
            window.position = new Rect(screenX, screenY, 400, 500);
            window.SetData(profileAnalyzerWindow, threadSelection, threadNames, threadUINames);
            window.Show();

            return window;
        }

        static public void CloseAll()
        {
            ThreadSelectionWindow window = GetWindow<ThreadSelectionWindow>("Threads");
            window.Close();
        }

        void CreateTable(ProfileAnalyzerWindow profileAnalyzerWindow, List<string> threadNames, List<string> threadUINames, ThreadSelection threadSelection)
        {
            if (m_ThreadTreeViewState == null)
                m_ThreadTreeViewState = new TreeViewState();

            m_ThreadMulticolumnHeaderState = ThreadTable.CreateDefaultMultiColumnHeaderState(700);

            var multiColumnHeader = new MultiColumnHeader(m_ThreadMulticolumnHeaderState);
            multiColumnHeader.SetSorting((int)ThreadTable.MyColumns.ThreadName, true);
            multiColumnHeader.ResizeToFit();
            m_ThreadTable = new ThreadTable(m_ThreadTreeViewState, multiColumnHeader, threadNames, threadUINames, threadSelection);
        }

        void SetData(ProfileAnalyzerWindow profileAnalyzerWindow, ThreadSelection threadSelection, List<string> threadNames, List<string> threadUINames)
        {
            m_ProfileAnalyzerWindow = profileAnalyzerWindow;
            CreateTable(profileAnalyzerWindow, threadNames, threadUINames, threadSelection);
        }

        private void OnDestroy()
        {
            m_ProfileAnalyzerWindow.SetThreadSelection(m_ThreadTable.GetThreadSelection());
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label("Select Thread : ", style);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear", GUILayout.Width(50)))
            {
                m_ThreadTable.ClearThreadSelection();
            }
            if (GUILayout.Button("Apply", GUILayout.Width(50)))
            {
                m_ProfileAnalyzerWindow.SetThreadSelection(m_ThreadTable.GetThreadSelection());
            }
            EditorGUILayout.EndHorizontal();

            if (m_ThreadTable != null)
            {
                Rect r = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true));
                m_ThreadTable.OnGUI(r);
            }

            EditorGUILayout.EndVertical();
        }

        private void OnLostFocus()
        {
            Close();
        }
    }
}
