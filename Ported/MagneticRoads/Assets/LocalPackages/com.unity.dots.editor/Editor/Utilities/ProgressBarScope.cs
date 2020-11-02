using System;
using UnityEditor;

namespace Unity.Entities.Editor
{
    class ProgressBarScope : IDisposable
    {
        private string m_Title;
        private static int s_Depth;
        private static string s_LastTitle, s_LastInfo;
        private static float s_LastProgress;

        public ProgressBarScope()
        {
            ++s_Depth;
        }

        public ProgressBarScope(string title, string info = null, float progress = float.MinValue)
        {
            ++s_Depth;
            m_Title = title;
            Display(title, info, progress, true);
        }

        static void Display(string title, string info, float progress, bool force)
        {
            var newTitle = title != null ? title : s_LastTitle != null ? s_LastTitle : string.Empty;
            var newInfo = info != null ? info : s_LastInfo != null ? s_LastInfo : string.Empty;
            var newProgress = progress != float.MinValue ? progress : s_LastProgress != float.MinValue ? s_LastProgress : 0f;

            // Only allow progress regression if the new value is greater than zero
            if (newProgress == 0f && s_LastProgress > 0f)
            {
                newProgress = s_LastProgress;
            }

            // If nothing changed, do not update progress bar uselessly
            if (!force && newTitle == s_LastTitle && newInfo == s_LastInfo && newProgress == s_LastProgress)
            {
                return;
            }

            EditorUtility.DisplayProgressBar(newTitle, newInfo, newProgress);

            s_LastTitle = newTitle;
            s_LastInfo = newInfo;
            s_LastProgress = newProgress;
        }

        public void Update(string info, float progress = float.MinValue)
        {
            Display(m_Title, info, progress, false);
        }

        public void Update(string title, string info, float progress = float.MinValue)
        {
            m_Title = title;
            Display(m_Title, info, progress, false);
        }

        public static void Restore()
        {
            if (s_Depth > 0 && s_LastTitle != null)
            {
                Display(s_LastTitle, s_LastInfo, s_LastProgress, true);
            }
        }

        public void Dispose()
        {
            if (--s_Depth == 0)
            {
                EditorUtility.ClearProgressBar();
                s_LastTitle = s_LastInfo = null;
                s_LastProgress = 0.0f;
            }
        }
    }
}
