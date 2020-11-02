using UnityEngine.Profiling;

namespace Unity.Entities.Editor
{
    class AverageRecorder
    {
        readonly Recorder m_Recorder;
        int m_FrameCount;
        int m_TotalNanoseconds;
        float m_LastReading;

        public AverageRecorder(Recorder recorder)
        {
            this.m_Recorder = recorder;
        }

        public void Update()
        {
            ++m_FrameCount;
            m_TotalNanoseconds += (int)m_Recorder.elapsedNanoseconds;
        }

        public float ReadMilliseconds()
        {
            if (m_FrameCount > 0)
            {
                m_LastReading = (m_TotalNanoseconds / 1e6f) / m_FrameCount;
                m_FrameCount = m_TotalNanoseconds = 0;
            }

            return m_LastReading;
        }
    }
}
