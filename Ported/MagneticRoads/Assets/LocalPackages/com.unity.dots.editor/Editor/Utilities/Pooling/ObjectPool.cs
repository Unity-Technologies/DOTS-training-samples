using System;
using System.Collections.Generic;

namespace Unity.Entities.Editor
{
    enum LifetimePolicy
    {
        Frame = 0,
        Permanent = 1
    }

    class ObjectPool<T> where T : class, new()
    {
        readonly HashSet<T> m_Inactive = new HashSet<T>();
        readonly HashSet<T> m_Active = new HashSet<T>();
        readonly Stack<T> m_Available = new Stack<T>();

        readonly List<T> m_NextFrame = new List<T>();

        readonly Action<T> m_ActionOnGet;
        readonly Action<T> m_ActionOnRelease;

        public int CountActive => m_Active.Count;
        public int CountInactive => m_Inactive.Count;

        public ObjectPool(Action<T> actionOnGet, Action<T> actionOnRelease)
        {
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;

            UnityEditor.EditorApplication.update += ManageNextFrameLifetime;
        }

        public T Get(LifetimePolicy lifetime)
        {
            T t = null;
            try
            {
                if (CountInactive > 0)
                {
                    t = m_Available.Pop();
                    m_Inactive.Remove(t);
                }
                else
                {
                    t = new T();
                }
                KeepTrackOfLifetime(t, lifetime);

                m_Active.Add(t);
                m_ActionOnGet?.Invoke(t);
            }
            catch (InvalidOperationException)
            {
                if (null != t)
                {
                    // At this point, a list was created, might as well take advantage of it.
                    m_Available.Push(t);
                    m_Inactive.Add(t);
                    m_Active.Remove(t);
                }
                throw;
            }

            return t;
        }

        public void Release(T element)
        {
            if (null == element)
            {
                throw new InvalidOperationException("Cannot return a null element to pool.");
            }

            if (!m_Active.Remove(element))
            {
                if (m_Inactive.Contains(element))
                {
                    throw new InvalidOperationException("Cannot return element to pool, it is already in the pool.");
                }
                else
                {
                    throw new InvalidOperationException("Cannot return element to pool, it is not owned by the pool.");
                }
            }

            m_Inactive.Add(element);
            m_ActionOnRelease?.Invoke(element);
            m_Available.Push(element);
        }

        void KeepTrackOfLifetime(T t, LifetimePolicy lifetime)
        {
            switch (lifetime)
            {
                case LifetimePolicy.Frame:
                    m_NextFrame.Add(t);
                    break;
                case LifetimePolicy.Permanent:
                    // Nothing to track
                    break;
                default:
                    throw new InvalidOperationException($"LifetimePolicy {lifetime} is not supported.");
            }
        }

        void ManageNextFrameLifetime()
        {
            try
            {
                foreach (var item in m_NextFrame)
                {
                    if (!m_Inactive.Contains(item))
                    {
                        throw new TimeoutException($"Pooled elements with the {LifetimePolicy.Frame} lifetime policy must be returned to the pool during the same frame.");
                    }
                }
            }
            // Do clear the list, so that the exception is not thrown every frame.
            finally
            {
                m_NextFrame.Clear();
            }
        }
    }
}
