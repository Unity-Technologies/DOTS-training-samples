using System.Diagnostics;

namespace UnityEditor.Build.Profiler
{
    public class BuildProfiler
    {
        Stopwatch[] m_Trackers;
        long[] m_CallCount;
        string[] m_Names;

        public BuildProfiler(int count)
        {
            m_Trackers = new Stopwatch[count];
            m_CallCount = new long[count];
            m_Names = new string[count];
            for (int i = 0; i < count; i++)
            {
                m_Trackers[i] = new Stopwatch();
                m_CallCount[i] = 0;
            }
        }

        public void Start(int index, string name)
        {
            if (m_Trackers == null)
                return;

            Debug.Assert(!m_Trackers[index].IsRunning);
            m_Trackers[index].Start();
            m_CallCount[index]++;
            m_Names[index] = name;
        }

        public void Stop(int index)
        {
            if (m_Trackers == null)
                return;

            Debug.Assert(m_Trackers[index].IsRunning);
            m_Trackers[index].Stop();
        }

        public void Print()
        {
            if (m_Trackers == null)
                return;

            string msg = "";
            for (int i = 0; i < m_Trackers.Length; i++)
            {
                Debug.Assert(!m_Trackers[i].IsRunning);
                msg += string.Format("Counter[{0}]\t{1}\t{2}\n", m_Names[i], m_CallCount[i], m_Trackers[i].ElapsedMilliseconds);
            }
            UnityEngine.Debug.Log(msg);
        }
    }
}