using System;
using System.Collections.Generic;
#if UNITY_2019_3_OR_NEWER
using UnityEngine.PlayerLoop;
#else
using UnityEngine.Experimental.PlayerLoop;
#endif
using UnityEngine.Scripting;

namespace Unity.Entities
{
    [DisableAutoCreation]
    [UnityEngine.ExecuteAlways]
    public class BeginInitializationEntityCommandBufferSystem : EntityCommandBufferSystem {}

    [DisableAutoCreation]
    [UnityEngine.ExecuteAlways]
    public class EndInitializationEntityCommandBufferSystem : EntityCommandBufferSystem {}

    public class InitializationSystemGroup : ComponentSystemGroup
    {
        [Preserve] public InitializationSystemGroup() {}

        BeginInitializationEntityCommandBufferSystem m_BeginEntityCommandBufferSystem;
        EndInitializationEntityCommandBufferSystem m_EndEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            m_BeginEntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            m_EndEntityCommandBufferSystem = World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
            m_systemsToUpdate.Add(m_BeginEntityCommandBufferSystem);
            m_systemsToUpdate.Add(m_EndEntityCommandBufferSystem);
        }

        public override void SortSystemUpdateList()
        {
            // Extract list of systems to sort (excluding built-in systems that are inserted at fixed points)
            var toSort = new List<ComponentSystemBase>(m_systemsToUpdate.Count - 2);
            foreach (var s in m_systemsToUpdate)
            {
                if (s is BeginInitializationEntityCommandBufferSystem ||
                    s is EndInitializationEntityCommandBufferSystem)
                {
                    continue;
                }
                toSort.Add(s);
            }
            m_systemsToUpdate = toSort;
            base.SortSystemUpdateList();
            // Re-insert built-in systems to construct the final list
            var finalSystemList = new List<ComponentSystemBase>(1 + m_systemsToUpdate.Count + 1);
            finalSystemList.Add(m_BeginEntityCommandBufferSystem);
            foreach (var s in m_systemsToUpdate)
                finalSystemList.Add(s);
            finalSystemList.Add(m_EndEntityCommandBufferSystem);
            m_systemsToUpdate = finalSystemList;
        }
    }

    [DisableAutoCreation]
    [UnityEngine.ExecuteAlways]
    public class BeginSimulationEntityCommandBufferSystem : EntityCommandBufferSystem {}

    [DisableAutoCreation]
    [UnityEngine.ExecuteAlways]
    public class EndSimulationEntityCommandBufferSystem : EntityCommandBufferSystem {}

    [DisableAutoCreation]
    public class LateSimulationSystemGroup : ComponentSystemGroup {}

    public class SimulationSystemGroup : ComponentSystemGroup
    {
        [Preserve] public SimulationSystemGroup() {}

        BeginSimulationEntityCommandBufferSystem m_BeginEntityCommandBufferSystem;
        LateSimulationSystemGroup m_lateSimulationGroup;
        EndSimulationEntityCommandBufferSystem m_EndEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            m_BeginEntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
            m_lateSimulationGroup = World.GetOrCreateSystem<LateSimulationSystemGroup>();
            m_EndEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            m_systemsToUpdate.Add(m_BeginEntityCommandBufferSystem);
            m_systemsToUpdate.Add(m_lateSimulationGroup);
            m_systemsToUpdate.Add(m_EndEntityCommandBufferSystem);
        }

        public override void SortSystemUpdateList()
        {
            // Extract list of systems to sort (excluding built-in systems that are inserted at fixed points)
            var toSort = new List<ComponentSystemBase>(m_systemsToUpdate.Count - 3);
            foreach (var s in m_systemsToUpdate)
            {
                if (s is BeginSimulationEntityCommandBufferSystem ||
                    s is LateSimulationSystemGroup ||
                    s is EndSimulationEntityCommandBufferSystem)
                {
                    continue;
                }
                toSort.Add(s);
            }
            m_systemsToUpdate = toSort;
            base.SortSystemUpdateList();
            m_lateSimulationGroup.SortSystemUpdateList(); // not handled by base-class sort call
            // Re-insert built-in systems to construct the final list
            var finalSystemList = new List<ComponentSystemBase>(1 + m_systemsToUpdate.Count + 2);
            finalSystemList.Add(m_BeginEntityCommandBufferSystem);
            foreach (var s in m_systemsToUpdate)
                finalSystemList.Add(s);
            finalSystemList.Add(m_lateSimulationGroup);
            finalSystemList.Add(m_EndEntityCommandBufferSystem);
            m_systemsToUpdate = finalSystemList;
        }
    }

    [DisableAutoCreation]
    [UnityEngine.ExecuteAlways]
    public class BeginPresentationEntityCommandBufferSystem : EntityCommandBufferSystem {}

    [DisableAutoCreation]
    [UnityEngine.ExecuteAlways]
    [Obsolete("please use BeginInitializationEntityCommandBufferSystem instead. (RemovedAfter 2019-11-06)")]
    public class EndPresentationEntityCommandBufferSystem : EntityCommandBufferSystem {}

    public class PresentationSystemGroup : ComponentSystemGroup
    {
        [Preserve] public PresentationSystemGroup() {}

        BeginPresentationEntityCommandBufferSystem m_BeginEntityCommandBufferSystem;

#pragma warning disable 0618
        // warning CS0618: 'EndPresentationEntityCommandBufferSystem' is obsolete
        EndPresentationEntityCommandBufferSystem m_EndEntityCommandBufferSystem;
#pragma warning restore 0618

        protected override void OnCreate()
        {
            m_BeginEntityCommandBufferSystem = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();

#pragma warning disable 0618
            // warning CS0618: 'EndPresentationEntityCommandBufferSystem' is obsolete
            m_EndEntityCommandBufferSystem = World.GetOrCreateSystem<EndPresentationEntityCommandBufferSystem>();
#pragma warning restore 0618

            m_systemsToUpdate.Add(m_BeginEntityCommandBufferSystem);
            m_systemsToUpdate.Add(m_EndEntityCommandBufferSystem);
        }

        public override void SortSystemUpdateList()
        {
            // Extract list of systems to sort (excluding built-in systems that are inserted at fixed points)
            var toSort = new List<ComponentSystemBase>(m_systemsToUpdate.Count - 2);
            foreach (var s in m_systemsToUpdate)
            {
                if (s is BeginPresentationEntityCommandBufferSystem ||
#pragma warning disable 0618
                    // warning CS0618: 'EndPresentationEntityCommandBufferSystem' is obsolete
                    s is EndPresentationEntityCommandBufferSystem)
#pragma warning restore 0618
                {
                    continue;
                }
                toSort.Add(s);
            }
            m_systemsToUpdate = toSort;
            base.SortSystemUpdateList();
            // Re-insert built-in systems to construct the final list
            var finalSystemList = new List<ComponentSystemBase>(1 + m_systemsToUpdate.Count + 1);
            finalSystemList.Add(m_BeginEntityCommandBufferSystem);
            foreach (var s in m_systemsToUpdate)
                finalSystemList.Add(s);
            finalSystemList.Add(m_EndEntityCommandBufferSystem);
            m_systemsToUpdate = finalSystemList;
        }
    }
}
