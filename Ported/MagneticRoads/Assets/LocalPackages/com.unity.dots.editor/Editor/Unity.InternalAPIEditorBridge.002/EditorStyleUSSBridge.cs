using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Unity.Editor.Bridge
{
    internal class EditorStyleUSSBridge
    {
        public static GUIStyle FromUSS(string ussStyleRuleName, string ussInPlaceStyleOverride = null, GUISkin srcSkin = null)
        {
            return EditorStyles.FromUSS(ussStyleRuleName);
        }

        public static GUIStyle ApplyUSS(GUIStyle style, string ussStyleRuleName, string ussInPlaceStyleOverride = null)
        {
            return EditorStyles.ApplyUSS(style, ussStyleRuleName, ussInPlaceStyleOverride);
        }

        public static void RepaintAllViews()
        {
            InternalEditorUtility.RepaintAllViews();
        }

        public static void RequestScriptReload()
        {
            EditorUtility.RequestScriptReload();
        }
    }
}
