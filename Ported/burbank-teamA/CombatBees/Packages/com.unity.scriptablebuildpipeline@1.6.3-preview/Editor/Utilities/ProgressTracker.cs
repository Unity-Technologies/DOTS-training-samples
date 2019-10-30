using System;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UnityEditor.Build.Pipeline.Utilities
{
    public class ProgressTracker : IProgressTracker, IDisposable
    {
        const long k_TicksPerSecond = 10000000;

        public int TaskCount { get; set; }

        public float Progress { get { return CurrentTask / (float)TaskCount; } }

        public uint UpdatesPerSecond
        {
            get { return (uint)(k_TicksPerSecond / UpdateFrequency); }
            set { UpdateFrequency = k_TicksPerSecond / Math.Max(value, 1); }
        }

        bool m_Disposed = false;

        protected int CurrentTask { get; set; }

        protected string CurrentTaskTitle { get; set; }

        protected long TimeStamp { get; set; }

        protected long UpdateFrequency { get; set; }

        public ProgressTracker()
        {
            CurrentTask = 0;
            CurrentTaskTitle = "";
            TimeStamp = 0;
            UpdateFrequency = k_TicksPerSecond / 100;
        }

        public virtual bool UpdateTask(string taskTitle)
        {
            CurrentTask++;
            CurrentTaskTitle = taskTitle;
            TimeStamp = 0;
            return !EditorUtility.DisplayCancelableProgressBar(CurrentTaskTitle, "", Progress);
        }

        public virtual bool UpdateInfo(string taskInfo)
        {
            var currentTicks = DateTime.Now.Ticks;
            if (currentTicks - TimeStamp < UpdateFrequency)
                return true;

            TimeStamp = currentTicks;
            return !EditorUtility.DisplayCancelableProgressBar(CurrentTaskTitle, taskInfo, Progress);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_Disposed)
                return;

            if (disposing)
                EditorUtility.ClearProgressBar();

            m_Disposed = true;
        }
    }
}
