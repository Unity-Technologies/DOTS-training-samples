using UnityEditor;
using UnityEngine;

namespace Unity.Editor.Bridge
{
    static class EditorGUIBridge
    {
        public static float indent => EditorGUI.indent;
        public static GUIContent mixedValueContent => EditorGUI.mixedValueContent;
    }
}
