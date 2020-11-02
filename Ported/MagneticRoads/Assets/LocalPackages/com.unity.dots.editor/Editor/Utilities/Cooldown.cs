using System;

namespace Unity.Entities.Editor
{
    class Cooldown
    {
        readonly TimeSpan m_Duration;
        DateTime m_LastExecution = DateTime.MinValue;

        public Cooldown(TimeSpan duration)
        {
            m_Duration = duration;
        }

        public bool Update(DateTime now)
        {
            if (now - m_LastExecution >= m_Duration)
            {
                m_LastExecution = now;
                return true;
            }

            return false;
        }
    }
}
