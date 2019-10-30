using System.Text.RegularExpressions;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UnityEditor.Build.Pipeline.Utilities
{
    public static class TrackerExtensions
    {
        public static string HumanReadable(this string camelCased)
        {
            return Regex.Replace(camelCased, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
        }

        public static bool UpdateTaskUnchecked(this IProgressTracker tracker, string taskTitle)
        {
            return tracker == null || tracker.UpdateTask(taskTitle);
        }

        public static bool UpdateInfoUnchecked(this IProgressTracker tracker, string taskInfo)
        {
            return tracker == null || tracker.UpdateInfo(taskInfo);
        }
    }
}