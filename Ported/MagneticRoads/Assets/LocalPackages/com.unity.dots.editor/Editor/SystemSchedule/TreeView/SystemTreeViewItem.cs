using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Editor.Bridge;
using Unity.Scenes;

namespace Unity.Entities.Editor
{
    class SystemTreeViewItem : ITreeViewItem, IPoolable
    {
        readonly List<ITreeViewItem> m_CachedChildren = new List<ITreeViewItem>();
        public IPlayerLoopNode Node;
        public PlayerLoopSystemGraph Graph;
        public World World;

        public SelectedSystem selectedSystem
        {
            get
            {
                if (Node is ISelectedSystemNode systemSelectionNode)
                    return systemSelectionNode.SelectedSystem;

                return null;
            }
        }

        public bool HasChildren => Node.Children.Count > 0;

        public string GetSystemName(World world = null)
        {
            if (world == null ||
                (Node is ISelectedSystemNode selectionSystemNode && selectionSystemNode.SelectedSystem.World.Name != world.Name))
            {
                return Node.NameWithWorld;
            }

            return Node?.Name;
        }

        public bool GetParentState()
        {
            return Node.EnabledInHierarchy;
        }

        public void SetPlayerLoopSystemState(bool state)
        {
            Node.Enabled = state;
        }

        public void SetSystemState(bool state)
        {
            if (Node.Enabled == state)
                return;

            Node.Enabled = state;
            EditorUpdateUtility.EditModeQueuePlayerLoopUpdate();
        }

        public unsafe string GetEntityMatches()
        {
            if (HasChildren) // Group system do not need entity matches.
                return string.Empty;

            var ptr = selectedSystem.StatePointer;
            if (ptr == null)
                return string.Empty;

            var matchedEntityCount = string.Empty;
            if (!Node.Enabled || !NodeParentsAllEnabled(Node))
            {
                matchedEntityCount = Constants.SystemSchedule.k_Dash;
            }
            else
            {
                var entityQueries = ptr->EntityQueries;
                var entityCountSum = 0;
                for (var i = 0; i < entityQueries.length; i++)
                {
                    entityCountSum += entityQueries[i].CalculateEntityCount();
                }

                matchedEntityCount = entityCountSum.ToString();
            }

            return matchedEntityCount;
        }

        float GetAverageRunningTime(SelectedSystem selectedSystem, SelectedSystem parentSelectedSystem)
        {
            if (selectedSystem.Managed != null && selectedSystem.Managed is ComponentSystemGroup systemGroup)
            {
                if (systemGroup.Systems != null)
                {
                    var managedChildSystemsSum = systemGroup.Systems.Sum(child => GetAverageRunningTime(child, systemGroup));

                    // unmanaged system
                    var unmanagedChildSystems = systemGroup.UnmanagedSystems;
                    var unmanagedChildSystemSum = 0.0f;
                    for (var i = 0; i < unmanagedChildSystems.length; i++)
                    {
                        unmanagedChildSystemSum += GetAverageRunningTime(new SelectedSystem(unmanagedChildSystems[i], systemGroup.World), systemGroup);
                    }

                    return managedChildSystemsSum + unmanagedChildSystemSum;
                }
            }
            else
            {
                var recorderKey = new PlayerLoopSystemGraph.RecorderKey
                {
                    World = selectedSystem.World,
                    Group = parentSelectedSystem.Managed as ComponentSystemGroup,
                    selectedSystem = selectedSystem
                };

                return Graph.RecordersBySystem.TryGetValue(recorderKey, out var recorder) ? recorder.ReadMilliseconds() : 0.0f;
            }

            return -1;
        }

        public string GetRunningTime()
        {
            var totalTime = string.Empty;

            if (Node is IPlayerLoopSystemData)
                return string.Empty;

            if (children.Any())
            {
                totalTime = !Node.Enabled || !NodeParentsAllEnabled(Node)
                    ? Constants.SystemSchedule.k_Dash
                    : Node.Children.OfType<ISelectedSystemNode>().Sum(child => GetAverageRunningTime(child.SelectedSystem, selectedSystem)).ToString("f2");
            }
            else
            {
                if (Node.IsRunning && Node is ISelectedSystemNode data && Node.Parent is ComponentGroupNode componentGroupNode)
                {
                    var parentSystem = componentGroupNode.SelectedSystem;
                    totalTime = !Node.Enabled || !NodeParentsAllEnabled(Node)
                        ? Constants.SystemSchedule.k_Dash
                        : GetAverageRunningTime(data.SelectedSystem, parentSystem).ToString("f2");
                }
                else
                {
                    return Constants.SystemSchedule.k_Dash;
                }
            }

            return totalTime;
        }

        bool NodeParentsAllEnabled(IPlayerLoopNode node)
        {
            if (node.Parent != null)
            {
                if (!node.Parent.Enabled) return false;
                if (!NodeParentsAllEnabled(node.Parent)) return false;
            }

            return true;
        }

        public int id => Node.Hash;
        public ITreeViewItem parent { get; internal set; }

        public IEnumerable<ITreeViewItem> children => m_CachedChildren;

        bool ITreeViewItem.hasChildren => HasChildren;

        public void AddChild(ITreeViewItem child)
        {
            throw new NotImplementedException();
        }

        public void AddChildren(IList<ITreeViewItem> children)
        {
            throw new NotImplementedException();
        }

        public void RemoveChild(ITreeViewItem child)
        {
            throw new NotImplementedException();
        }

        public void PopulateChildren(SystemScheduleSearchBuilder.ParseResult searchFilter, List<Type> systemDependencyList = null)
        {
            m_CachedChildren.Clear();

            foreach (var child in Node.Children)
            {
                if (!child.ShowForWorld(World))
                    continue;

                // Filter systems by system name, component types, system dependencies.
                if (!searchFilter.IsEmpty && !FilterSystem(child, searchFilter, systemDependencyList))
                    continue;

                var item = SystemSchedulePool.GetSystemTreeViewItem(Graph, child, this, World);
                m_CachedChildren.Add(item);
            }
        }

        static bool FilterSystem(IPlayerLoopNode node, SystemScheduleSearchBuilder.ParseResult searchFilter, List<Type> systemDependencyList)
        {
            switch (node)
            {
                case SelectedSelectedSystemNode baseNode:
                {
                    return FilterBaseSystem(baseNode.SelectedSystem, searchFilter, systemDependencyList);
                }

                case ComponentGroupNode groupNode:
                {
                    // Deal with group node dependencies first.
                    if (FilterBaseSystem(groupNode.SelectedSystem, searchFilter, systemDependencyList))
                        return true;

                    // Then their children.
                    if (groupNode.Children.Any(child => FilterSystem(child, searchFilter, systemDependencyList)))
                        return true;

                    break;
                }
            }

            return false;
        }

        static bool FilterBaseSystem(SelectedSystem selectedSystem, SystemScheduleSearchBuilder.ParseResult searchFilter, List<Type> systemDependencyList)
        {
            if (null == selectedSystem)
                return false;

            var systemType = selectedSystem.GetSystemType();

            if (searchFilter.ComponentNames.Any())
            {
                foreach (var componentName in searchFilter.ComponentNames)
                {
                    if (!EntityQueryUtility.ContainsThisComponentType(selectedSystem, componentName))
                        return false;
                }
            }

            if (searchFilter.DependencySystemNames.Any() && systemDependencyList != null && !systemDependencyList.Contains(systemType))
            {
                return false;
            }

            if (searchFilter.SystemNames.Any())
            {
                foreach (var singleSystemName in searchFilter.SystemNames)
                {
                    if (systemType.Name.IndexOf(singleSystemName, StringComparison.OrdinalIgnoreCase) < 0)
                        return false;
                }
            }

            return true;
        }

        public void Reset()
        {
            World = null;
            Graph = null;
            Node = null;
            parent = null;
            m_CachedChildren.Clear();
        }

        public void ReturnToPool()
        {
            foreach (var child in m_CachedChildren.OfType<SystemTreeViewItem>())
            {
                child.ReturnToPool();
            }

            SystemSchedulePool.ReturnToPool(this);
        }
    }
}
