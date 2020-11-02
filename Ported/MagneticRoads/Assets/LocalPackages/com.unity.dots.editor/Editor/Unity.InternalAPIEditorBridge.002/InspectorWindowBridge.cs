using UnityEditor;

namespace Unity.Editor.Bridge
{
    static class InspectorWindowBridge
    {
        public static void RepaintAllInspectors() => InspectorWindow.RepaintAllInspectors();

        public static void ReloadAllInspectors()
        {
#if UNITY_2020_1_OR_NEWER
            InspectorWindow.RefreshInspectors();
#endif
        }
    }
}
