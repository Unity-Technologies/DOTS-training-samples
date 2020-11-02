using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.LowLevel;
using UnityEngine.Profiling;

namespace Unity.Entities.Editor
{
    using SystemWrapper = ScriptBehaviourUpdateOrder.DummyDelegateWrapper;

    class PlayerLoopSystemGraph
    {
        static int s_RefCount;
        static readonly Cooldown k_Cooldown = new Cooldown(TimeSpan.FromSeconds(1));

        // We only support one graph at a time for now, so this is encapsulated here.
        public static PlayerLoopSystemGraph Current { get; private set; } = new PlayerLoopSystemGraph();
        public static event Action OnGraphChanged;

        public static void Register()
        {
            if (s_RefCount++ != 0)
                return;

            ValidateCurrentGraph(force: true);
            EditorApplication.update += PeriodicValidateCurrentGraph;
        }

        public static void Unregister()
        {
            if (s_RefCount == 0 || --s_RefCount > 0)
                return;

            // ReSharper disable DelegateSubtraction
            EditorApplication.update -= PeriodicValidateCurrentGraph;
            // ReSharper restore DelegateSubtraction
        }

        static void PeriodicValidateCurrentGraph()
            => ValidateCurrentGraph(force: false);

        internal static void ValidateCurrentGraph(bool force = false)
        {
            var shouldRun = k_Cooldown.Update(DateTime.Now);
            if (!force && !shouldRun)
                return;

            foreach (var recorder in Current.RecordersBySystem.Values)
            {
                recorder.Update();
            }

            var graph = new PlayerLoopSystemGraph();
            ParsePlayerLoopSystem(PlayerLoop.GetCurrentPlayerLoop(), graph);
            if (!DidChange(Current, graph))
            {
                graph.Reset();
                return;
            }

            Current.Reset();
            Current = graph;
            OnGraphChanged?.Invoke();
        }

        static bool DidChange(PlayerLoopSystemGraph lhs, PlayerLoopSystemGraph rhs)
        {
            if (lhs.Roots.Count != rhs.Roots.Count)
                return true;

            for (var i = 0; i < lhs.Roots.Count; ++i)
            {
                if (DidChange(lhs.Roots[i], rhs.Roots[i]))
                    return true;
            }

            return false;
        }

        static bool DidChange(IPlayerLoopNode lhs, IPlayerLoopNode rhs)
        {
            if (lhs.Parent?.Hash != rhs.Parent?.Hash)
                return true;

            if (lhs.Children.Count != rhs.Children.Count)
                return true;

            if (lhs.Hash != rhs.Hash)
                return true;

            for (var i = 0; i < lhs.Children.Count; ++i)
            {
                if (DidChange(lhs.Children[i], rhs.Children[i]))
                    return true;
            }

            return false;
        }

        public struct RecorderKey
        {
            public World World;
            public ComponentSystemGroup Group;
            public SelectedSystem selectedSystem;
        }

        public readonly List<IPlayerLoopNode> Roots = new List<IPlayerLoopNode>();

        public readonly Dictionary<RecorderKey, AverageRecorder> RecordersBySystem = new Dictionary<RecorderKey, AverageRecorder>();

        public readonly List<SelectedSystem> AllSystems = new List<SelectedSystem>();

        public void Reset()
        {
            foreach (var root in Roots)
            {
                root.ReturnToPool();
            }
            Roots.Clear();
            RecordersBySystem.Clear();
            AllSystems.Clear();
        }

        // Parse through the player loop system to get all system list and their parent-children relationship,
        // which will be used to build the treeview.
        public static void ParsePlayerLoopSystem(PlayerLoopSystem rootPlayerLoopSystem, PlayerLoopSystemGraph graph)
        {
            graph.Reset();
            Parse(rootPlayerLoopSystem, graph);
        }

        static void Parse(PlayerLoopSystem playerLoopSystem, PlayerLoopSystemGraph graph, IPlayerLoopNode parent = null)
        {
            // The integration of `ComponentSystemBase` into the player loop is done through a wrapper type.
            // If the target of the player loop system is the wrapper type, we will parse this as a `ComponentSystemBase`.
            if (null != playerLoopSystem.updateDelegate && playerLoopSystem.updateDelegate.Target is SystemWrapper wrapper)
            {
                Parse(wrapper.System, graph, parent);
                return;
            }

            // Add the player loop system to the graph if it is not the root one.
            if (null != playerLoopSystem.type)
            {
                var playerLoopSystemNode = Pool<PlayerLoopSystemNode>.GetPooled();
                playerLoopSystemNode.Value = playerLoopSystem;
                var node = playerLoopSystemNode;
                AddToGraph(graph, node, parent);
                parent = node;
            }

            if (null == playerLoopSystem.subSystemList)
                return;

            foreach (var subSystem in playerLoopSystem.subSystemList)
            {
                Parse(subSystem, graph, parent);
            }
        }

        static void Parse(SelectedSystem selectedSystem, PlayerLoopSystemGraph graph, IPlayerLoopNode parent = null)
        {
            IPlayerLoopNode node;

            graph.AllSystems.Add(selectedSystem);

            if (selectedSystem.Managed != null && selectedSystem.Managed is ComponentSystemGroup group)
            {
                var groupNode = Pool<ComponentGroupNode>.GetPooled();
                groupNode.Value = group;
                node = groupNode;

                ref var updateSystemList = ref group.m_MasterUpdateList;
                for (int i = 0, count = updateSystemList.length; i < count; ++i)
                {
                    var updateIndex = updateSystemList[i];
                    if (updateIndex.IsManaged)
                    {
                        var child = group.Systems[updateIndex.Index];
                        Parse(child, graph, node);
                    }
                    else
                    {
                        var child = group.UnmanagedSystems[updateIndex.Index];
                        Parse(new SelectedSystem(child, group.World), graph, node);
                    }
                }
            }
            else
            {
                var systemNode = Pool<SelectedSelectedSystemNode>.GetPooled();
                systemNode.Value = selectedSystem;

                node = systemNode;

                var recorder = Recorder.Get($"{selectedSystem.World?.Name ?? "none"} {selectedSystem.GetSystemType().FullName}");
                var recorderKey = new RecorderKey
                {
                    World = selectedSystem.World,
                    Group = (ComponentSystemGroup)((ComponentGroupNode)parent).SelectedSystem.Managed,
                    selectedSystem = selectedSystem
                };

                if (!graph.RecordersBySystem.ContainsKey(recorderKey))
                {
                    graph.RecordersBySystem.Add(recorderKey, new AverageRecorder(recorder));
                }
                else
                {
                    UnityEngine.Debug.LogError("System added twice: " + selectedSystem);
                }

                recorder.enabled = true;
            }

            AddToGraph(graph, node, parent);
        }

        static void AddToGraph(PlayerLoopSystemGraph graph, IPlayerLoopNode node, IPlayerLoopNode parent = null)
        {
            if (null == parent)
            {
                graph.Roots.Add(node);
            }
            else
            {
                node.Parent = parent;
                parent.Children.Add(node);
            }
        }
    }
}
