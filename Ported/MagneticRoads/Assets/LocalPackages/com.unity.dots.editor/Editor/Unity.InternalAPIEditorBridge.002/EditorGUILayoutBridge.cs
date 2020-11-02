using UnityEditor;
using UnityEngine;

namespace Unity.Editor.Bridge
{
    static class EditorGUILayoutBridge
    {
        public static Rect s_LastRect
        {
            get => EditorGUILayout.s_LastRect;
            set => EditorGUILayout.s_LastRect = value;
        }
    }
}
