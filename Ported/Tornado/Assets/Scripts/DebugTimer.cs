using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

struct DebugTimer : IDisposable
{
    private bool m_Disposed;
    private string m_Name;
    private Stopwatch m_Timer;
    private double m_MinTime;

    public double timeMs => m_Timer.Elapsed.TotalMilliseconds;

    public DebugTimer(string name, double minTimeMs = 0)
    {
        m_Disposed = false;
        m_Name = name;
        m_Timer = Stopwatch.StartNew();
        m_MinTime = minTimeMs;
    }

    public void Dispose()
    {
        if (m_Disposed)
            return;
        m_Disposed = true;

        m_Timer.Stop();
        if (!String.IsNullOrEmpty(m_Name) && timeMs >= m_MinTime)
            Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, $"{m_Name} took {timeMs:F2} ms", 0, 0, 0);
    }
}
