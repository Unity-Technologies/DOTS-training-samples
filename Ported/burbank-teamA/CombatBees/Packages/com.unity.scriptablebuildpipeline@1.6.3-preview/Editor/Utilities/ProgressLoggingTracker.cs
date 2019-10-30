using System;

namespace UnityEditor.Build.Pipeline.Utilities
{
    public class ProgressLoggingTracker : ProgressTracker
    {
        public ProgressLoggingTracker()
        {
            BuildLogger.Log(string.Format("[{0}] Progress Tracker Started.", DateTime.Now.ToString()));
        }

        public override bool UpdateTask(string taskTitle)
        {
            BuildLogger.Log(string.Format("[{0}] {1:P2} Running Task: '{2}'", DateTime.Now.ToString(), Progress.ToString(), taskTitle));
            return base.UpdateTask(taskTitle);
        }

        public override bool UpdateInfo(string taskInfo)
        {
            BuildLogger.Log(string.Format("[{0}] {1:P2} Running Task: '{2}' Information: '{3}'", DateTime.Now.ToString(), Progress.ToString(), CurrentTaskTitle, taskInfo));
            return base.UpdateInfo(taskInfo);
        }

        protected override void Dispose(bool disposing)
        {
            BuildLogger.Log(string.Format("[{0}] Progress Tracker Completed.", DateTime.Now.ToString()));
            base.Dispose(disposing);
        }
    }
}
