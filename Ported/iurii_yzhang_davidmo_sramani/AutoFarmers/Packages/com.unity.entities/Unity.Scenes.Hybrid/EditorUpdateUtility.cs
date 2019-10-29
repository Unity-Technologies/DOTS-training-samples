using UnityEditor;
using UnityEngine;

namespace Unity.Scenes
{
    static class EditorUpdateUtility
    {
#if UNITY_EDITOR
        public static bool DidRequest = false;
        public static void EditModeQueuePlayerLoopUpdate()
        {
            if (!Application.isPlaying && !DidRequest)
            {
                DidRequest = true;
                EditorApplication.QueuePlayerLoopUpdate();
                EditorApplication.update += EditorUpdate;
            }
        }
        static void EditorUpdate()
        {
            DidRequest = false;
            EditorApplication.update -= EditorUpdate;
            EditorApplication.QueuePlayerLoopUpdate();
        }
#else    
        public static void EditModeQueuePlayerLoopUpdate() { }
#endif
    }
}
