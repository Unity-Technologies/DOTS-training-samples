using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.PerformanceTesting
{
    class TestListTableItem : TreeViewItem
    {
        public PerformanceTest test;
        public double deviation;
        public double standardDeviation;
        public double median;

        public TestListTableItem(int id, int depth, string displayName, PerformanceTest test) : base(id, depth,
            displayName)
        {
            this.test = test;

            deviation = 0f;
            if (test != null)
            {
                foreach (var sample in test.SampleGroups)
                {
                    if (sample.StandardDeviation > deviation)
                        standardDeviation = sample.StandardDeviation;

                    double thisDeviation = sample.StandardDeviation / sample.Median;
                    if (thisDeviation > deviation)
                    {
                        deviation = thisDeviation;
                        median = sample.Median;
                    }
                }
            }
        }
    }

    class TestListTable : TreeView
    {
        TestReportWindow m_testReportWindow;

        const float kRowHeights = 20f;
        readonly List<TreeViewItem> m_Rows = new List<TreeViewItem>(100);

        // All columns
        public enum MyColumns
        {
            Name,
            SampleCount,
            StandardDeviation,
            Deviation,
            Median
        }

        public enum SortOption
        {
            Name,
            SampleCount,
            StandardDeviation,
            Deviation,
            Median
        }

        // Sort options per column
        SortOption[] m_SortOptions =
        {
            SortOption.Name,
            SortOption.SampleCount,
            SortOption.StandardDeviation,
            SortOption.Deviation,
            SortOption.Median
        };


        public TestListTable(TreeViewState state, MultiColumnHeader multicolumnHeader,
            TestReportWindow testReportWindow) : base(state, multicolumnHeader)
        {
            m_testReportWindow = testReportWindow;

            Assert.AreEqual(m_SortOptions.Length, Enum.GetValues(typeof(MyColumns)).Length,
                "Ensure number of sort options are in sync with number of MyColumns enum values");

            // Custom setup
            rowHeight = kRowHeights;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            customFoldoutYOffset =
                (kRowHeights - EditorGUIUtility.singleLineHeight) *
                0.5f; // center foldout in the row since we also center content. See RowGUI
            // extraSpaceBeforeIconAndLabel = 0;
            multicolumnHeader.sortingChanged += OnSortingChanged;

            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            int idForhiddenRoot = -1;
            int depthForHiddenRoot = -1;
            TestListTableItem root = new TestListTableItem(idForhiddenRoot, depthForHiddenRoot, "root", null);

            var results = m_testReportWindow.GetResults();
            if (results != null)
            {
                int index = 0;
                foreach (var result in results.Results)
                {
                    var item = new TestListTableItem(index, 0, result.TestName, result);
                    root.AddChild(item);

                    // Maintain index to map to main markers
                    index += 1;
                }
            }

            return root;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            m_Rows.Clear();

            if (rootItem != null && rootItem.children != null)
            {
                foreach (TestListTableItem node in rootItem.children)
                {
                    m_Rows.Add(node);
                }
            }

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

            // Update the data with the sorted content
            rows.Clear();
            foreach (TestListTableItem node in rootItem.children)
            {
                rows.Add(node);
            }

            Repaint();
        }

        void SortByMultipleColumns()
        {
            var sortedColumns = multiColumnHeader.state.sortedColumns;

            if (sortedColumns.Length == 0)
            {
                return;
            }

            var myTypes = rootItem.children.Cast<TestListTableItem>();
            var orderedQuery = InitialOrder(myTypes, sortedColumns);
            for (int i = 1; i < sortedColumns.Length; i++)
            {
                SortOption sortOption = m_SortOptions[sortedColumns[i]];
                bool ascending = multiColumnHeader.IsSortedAscending(sortedColumns[i]);

                switch (sortOption)
                {
                    case SortOption.Name:
                        orderedQuery = orderedQuery.ThenBy(l => l.displayName, ascending);
                        break;
                    case SortOption.SampleCount:
                        orderedQuery = orderedQuery.ThenBy(l => l.test.SampleGroups.Count, ascending);
                        break;
                    case SortOption.Deviation:
                        orderedQuery = orderedQuery.ThenBy(l => l.deviation, ascending);
                        break;
                    case SortOption.StandardDeviation:
                        orderedQuery = orderedQuery.ThenBy(l => l.standardDeviation, ascending);
                        break;
                    case SortOption.Median:
                        orderedQuery = orderedQuery.ThenBy(l => l.median, ascending);
                        break;
                }
            }

            rootItem.children = orderedQuery.Cast<TreeViewItem>().ToList();
        }

        IOrderedEnumerable<TestListTableItem> InitialOrder(IEnumerable<TestListTableItem> myTypes, int[] history)
        {
            SortOption sortOption = m_SortOptions[history[0]];
            bool ascending = multiColumnHeader.IsSortedAscending(history[0]);
            switch (sortOption)
            {
                case SortOption.Name:
                    return myTypes.Order(l => l.displayName, ascending);
                case SortOption.SampleCount:
                    return myTypes.Order(l => l.test.SampleGroups.Count, ascending);
                case SortOption.Deviation:
                    return myTypes.Order(l => l.deviation, ascending);
                case SortOption.StandardDeviation:
                    return myTypes.Order(l => l.standardDeviation, ascending);
                case SortOption.Median:
                    return myTypes.Order(l => l.median, ascending);
                default:
                    Assert.IsTrue(false, "Unhandled enum");
                    break;
            }

            // default
            return myTypes.Order(l => l.displayName, ascending);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (TestListTableItem) args.item;

            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, (MyColumns) args.GetColumn(i), ref args);
            }
        }

        void CellGUI(Rect cellRect, TestListTableItem item, MyColumns column, ref RowGUIArgs args)
        {
            // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
            CenterRectUsingSingleLineHeight(ref cellRect);

            switch (column)
            {
                case MyColumns.Name:
                {
                    args.rowRect = cellRect;
                    base.RowGUI(args);
                }
                    break;
                case MyColumns.SampleCount:
                    EditorGUI.LabelField(cellRect, string.Format("{0}", item.test.SampleGroups.Count));
                    break;
                case MyColumns.Deviation:
                    EditorGUI.LabelField(cellRect, string.Format("{0:f2}", item.deviation));
                    break;
                case MyColumns.StandardDeviation:
                    EditorGUI.LabelField(cellRect, string.Format("{0:f2}", item.standardDeviation));
                    break;
                case MyColumns.Median:
                    EditorGUI.LabelField(cellRect, string.Format("{0:f2}", item.median));
                    break;
            }
        }


        // Misc
        //--------

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
        {
            var columnList = new List<MultiColumnHeaderState.Column>();
            columnList.Add(new MultiColumnHeaderState.Column
            {
                headerContent = new GUIContent("Name"),
                headerTextAlignment = TextAlignment.Left,
                sortedAscending = true,
                sortingArrowAlignment = TextAlignment.Left,
                width = 600,
                minWidth = 100,
                autoResize = false,
                allowToggleVisibility = false
            });
            string[] names = {"Sample Groups", "Standard Deviation", "Deviation", "Median"};
            foreach (var name in names)
            {
                var column = new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent(name),
                    headerTextAlignment = TextAlignment.Left,
                    sortedAscending = true,
                    sortingArrowAlignment = TextAlignment.Left,
                    width = 100,
                    minWidth = 50,
                    autoResize = true
                };
                columnList.Add(column);
            }

            ;
            var columns = columnList.ToArray();

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(MyColumns)).Length,
                "Number of columns should match number of enum values: You probably forgot to update one of them.");

            var state = new MultiColumnHeaderState(columns);
            state.visibleColumns = new int[]
            {
                (int) MyColumns.Name,
                (int) MyColumns.SampleCount,
                (int) MyColumns.Deviation,
                (int) MyColumns.StandardDeviation,
                (int) MyColumns.Median
            };
            return state;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);

            if (selectedIds.Count > 0)
                m_testReportWindow.SelectTest(selectedIds[0]);
        }
    }

    static class MyExtensionMethods
    {
        public static IOrderedEnumerable<T> Order<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector,
            bool ascending)
        {
            if (ascending)
            {
                return source.OrderBy(selector);
            }
            else
            {
                return source.OrderByDescending(selector);
            }
        }

        public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> selector,
            bool ascending)
        {
            if (ascending)
            {
                return source.ThenBy(selector);
            }
            else
            {
                return source.ThenByDescending(selector);
            }
        }
    }
}