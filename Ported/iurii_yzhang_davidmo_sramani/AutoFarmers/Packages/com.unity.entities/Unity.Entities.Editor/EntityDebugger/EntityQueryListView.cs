using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Unity.Entities.Editor
{
    internal delegate void SetEntityListSelection(EntityListQuery query);

    internal class EntityQueryListView : TreeView {
        private static Dictionary<ComponentSystemBase, List<EntityQueryDesc>> queryDescsBySystem = new Dictionary<ComponentSystemBase, List<EntityQueryDesc>>();

        private readonly Dictionary<int, EntityQuery> queriesById = new Dictionary<int, EntityQuery>();
        private readonly Dictionary<int, EntityQueryDesc> queryDescsById = new Dictionary<int, EntityQueryDesc>();
        private readonly Dictionary<int, EntityQueryGUIControl> controlsById = new Dictionary<int, EntityQueryGUIControl>();

        public ComponentSystemBase SelectedSystem
        {
            get { return selectedSystem; }
            set
            {
                if (selectedSystem != value)
                {
                    selectedSystem = value;
                    Reload();
                }
            }
        }
        private ComponentSystemBase selectedSystem;

        private readonly WorldSelectionGetter getWorldSelection;
        private readonly SetEntityListSelection entityListSelectionCallback;

        private static TreeViewState GetStateForSystem(ComponentSystemBase system, List<TreeViewState> states, List<string> stateNames)
        {
            if (system == null)
                return new TreeViewState();

            var currentSystemName = system.GetType().FullName;

            var stateForCurrentSystem = states.Where((t, i) => stateNames[i] == currentSystemName).FirstOrDefault();
            if (stateForCurrentSystem != null)
                return stateForCurrentSystem;

            stateForCurrentSystem = new TreeViewState();
            if (system.EntityQueries != null && system.EntityQueries.Length > 0)
                stateForCurrentSystem.expandedIDs = new List<int> {1};
            states.Add(stateForCurrentSystem);
            stateNames.Add(currentSystemName);
            return stateForCurrentSystem;
        }

        public static EntityQueryListView CreateList(ComponentSystemBase system, List<TreeViewState> states, List<string> stateNames,
            SetEntityListSelection entityQuerySelectionCallback, WorldSelectionGetter worldSelectionGetter)
        {
            var state = GetStateForSystem(system, states, stateNames);
            return new EntityQueryListView(state, system, entityQuerySelectionCallback, worldSelectionGetter);
        }

        public EntityQueryListView(TreeViewState state, ComponentSystemBase system, SetEntityListSelection entityListSelectionCallback, WorldSelectionGetter worldSelectionGetter) : base(state)
        {
            this.getWorldSelection = worldSelectionGetter;
            this.entityListSelectionCallback = entityListSelectionCallback;
            selectedSystem = system;
            rowHeight += 1;
            showAlternatingRowBackgrounds = true;
            Reload();
        }

        public float Height { get; private set; }

        protected override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            return controlsById.ContainsKey(item.id) ? controlsById[item.id].Height + 2 : rowHeight;
        }

        private static List<EntityQueryDesc> GetQueryDescsForSystem(ComponentSystemBase system)
        {
            List<EntityQueryDesc> queryDescs;
            if (queryDescsBySystem.TryGetValue(system, out queryDescs))
                return queryDescs;

            queryDescs = new List<EntityQueryDesc>();

            var currentType = system.GetType();

            while (currentType != null)
            {
                foreach (var field in currentType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    if (field.FieldType == typeof(EntityQueryDesc))
                        queryDescs.Add(field.GetValue(system) as EntityQueryDesc);
                }

                currentType = currentType.BaseType;
            }

            return queryDescs;
        }

        protected override TreeViewItem BuildRoot()
        {
            queriesById.Clear();
            queryDescsById.Clear();
            controlsById.Clear();
            var currentId = 0;
            var root  = new TreeViewItem { id = currentId++, depth = -1, displayName = "Root" };
            if (getWorldSelection() == null)
            {
                root.AddChild(new TreeViewItem { id = currentId, displayName = "No world selected"});
            }
            else if (SelectedSystem == null)
            {
                root.AddChild(new TreeViewItem { id = currentId, displayName = "Null System"});
            }
            else
            {
                var queryDescs = GetQueryDescsForSystem(SelectedSystem);
                var entityManager = getWorldSelection().EntityManager;

                foreach (var queryDesc in queryDescs)
                {
                    var group = entityManager.CreateEntityQuery(queryDesc);
                    queryDescsById.Add(currentId, queryDesc);
                    queriesById.Add(currentId, group);

                    var groupItem = new TreeViewItem { id = currentId++ };
                    root.AddChild(groupItem);
                }
                if (SelectedSystem.EntityQueries != null)
                {
                    foreach (var query in SelectedSystem.EntityQueries)
                    {
                        queriesById.Add(currentId, query);

                        var queryItem = new TreeViewItem { id = currentId++ };
                        root.AddChild(queryItem);
                    }
                }
                if (queriesById.Count == 0)
                {
                    root.AddChild(new TreeViewItem { id = currentId, displayName = "No Entity Queries in System"});
                }
                else
                {
                    SetupDepthsFromParentsAndChildren(root);

                    foreach (var idQueryPair in queriesById)
                    {
                        var newControl = new EntityQueryGUIControl(idQueryPair.Value.GetQueryTypes(), idQueryPair.Value.GetReadAndWriteTypes(), true);
                        controlsById.Add(idQueryPair.Key, newControl);
                    }
                }
            }
            return root;
        }

        private float width;
        private const float kBorderWidth = 60f;

        public void SetWidth(float newWidth)
        {
            newWidth -= kBorderWidth;
            if (newWidth != width)
            {
                width = newWidth;
                foreach (var control in controlsById.Values)
                    control.UpdateSize(width);
            }
            RefreshCustomRowHeights();
            var height = 0f;
            foreach (var child in rootItem.children)
                height += GetCustomRowHeight(0, child);
            Height = height;
        }

        public override void OnGUI(Rect rect)
        {
            if (getWorldSelection()?.EntityManager.IsCreated == true)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    SetWidth(rect.width);
                }
                base.OnGUI(rect);
            }
        }

        protected void DrawCount(RowGUIArgs args)
        {
            EntityQuery entityQuery;
            if (queriesById.TryGetValue(args.item.id, out entityQuery))
            {
                var countString = entityQuery.CalculateEntityCount().ToString();
                DefaultGUI.LabelRightAligned(args.rowRect, countString, args.selected, args.focused);
            }
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            base.RowGUI(args);
            if (Event.current.type != EventType.Repaint || !controlsById.ContainsKey(args.item.id))
                return;

            var position = args.rowRect.position;
            position.x = GetContentIndent(args.item);
            position.y += 1;

            controlsById[args.item.id].OnGUI(position);

            DrawCount(args);
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (selectedIds.Count > 0)
            {
                EntityQuery entityQuery;
                if (queriesById.TryGetValue(selectedIds[0], out entityQuery))
                    entityListSelectionCallback(new EntityListQuery(entityQuery));
            }
            else
            {
                entityListSelectionCallback(null);
            }
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        public void SetEntityListSelection(EntityListQuery newListQuery)
        {
            if (newListQuery == null)
            {
                SetSelection(new List<int>());
                return;
            }
            if (newListQuery.Group != null)
            {
                foreach (var pair in queriesById)
                {
                    if (pair.Value == newListQuery.Group)
                    {
                        SetSelection(new List<int> {pair.Key});
                        return;
                    }
                }
            }
            else
            {
                foreach (var pair in queryDescsById)
                {
                    if (pair.Value == newListQuery.QueryDesc)
                    {
                        SetSelection(new List<int> {pair.Key});
                        return;
                    }
                }
            }
            SetSelection(new List<int>());
        }

        public void SetEntityQuerySelection(EntityQuery group)
        {
            SetSelection(new List<int>());
        }

        public void TouchSelection()
        {
            SetSelection(GetSelection(), TreeViewSelectionOptions.FireSelectionChanged);
        }

        public bool NeedsReload
        {
            get
            {
                var expectedGroupCount = SelectedSystem?.EntityQueries?.Length ?? 0;
                return expectedGroupCount != queriesById.Count;
            }

        }

        public void ReloadIfNecessary()
        {
            if (NeedsReload)
                Reload();
        }
    }
}
