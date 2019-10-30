using System;
using System.Collections.Generic;
#if !NET_DOTS
using System.Linq;
#endif

namespace Unity.Entities
{

    public abstract class ComponentSystemGroup : ComponentSystem
    {
        private bool m_systemSortDirty = false;
        protected List<ComponentSystemBase> m_systemsToUpdate = new List<ComponentSystemBase>();
        protected List<ComponentSystemBase> m_systemsToRemove = new List<ComponentSystemBase>();

        public virtual IEnumerable<ComponentSystemBase> Systems => m_systemsToUpdate;

        public void AddSystemToUpdateList(ComponentSystemBase sys)
        {
            if (sys != null)
            {
                if (this == sys)
                    throw new ArgumentException($"Can't add {TypeManager.SystemName(GetType())} to its own update list");

                // Check for duplicate Systems. Also see issue #1792
                if (m_systemsToUpdate.IndexOf(sys) >= 0)
                    return;

                m_systemsToUpdate.Add(sys);
                m_systemSortDirty = true;
            }
        }

        public void RemoveSystemFromUpdateList(ComponentSystemBase sys)
        {
            m_systemsToRemove.Add(sys);
        }

        public virtual void SortSystemUpdateList()
        {
            if (!m_systemSortDirty)
                return;
            m_systemSortDirty = false;

            foreach (var sys in m_systemsToUpdate)
            {
                if (TypeManager.IsSystemAGroup(sys.GetType()))
                {
                    ((ComponentSystemGroup)sys).SortSystemUpdateList();
                }
            }

            ComponentSystemSorter.Sort(m_systemsToUpdate, x => x.GetType(), this.GetType());
        }

#if UNITY_DOTSPLAYER
        public void RecursiveLogToConsole()
        {
            foreach (var sys in m_systemsToUpdate)
            {
                if (sys is ComponentSystemGroup)
                {
                    (sys as ComponentSystemGroup).RecursiveLogToConsole();
                }

                var index = TypeManager.GetSystemTypeIndex(sys.GetType());
                var names = TypeManager.SystemNames;
                var name = names[index];
                Debug.Log(name);
            }
        }

#endif

        protected override void OnStopRunning()
        {
        }

        internal override void OnStopRunningInternal()
        {
            OnStopRunning();

            foreach (var sys in m_systemsToUpdate)
            {
                if ((sys == null) || (!sys.m_PreviouslyEnabled)) continue;

                sys.m_PreviouslyEnabled = false;
                sys.OnStopRunningInternal();
            }
        }

        protected override void OnUpdate()
        {
            if (m_systemSortDirty)
                SortSystemUpdateList();

            foreach (var sys in m_systemsToUpdate)
            {
                try
                {
                    sys.Update();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                if (World.QuitUpdate)
                    break;
            }

            if (m_systemsToRemove.Count > 0)
            {
                foreach (var sys in m_systemsToRemove)
                    m_systemsToUpdate.Remove(sys);
                m_systemSortDirty = true;
                m_systemsToRemove.Clear();
            }
        }
    }
}
