using UnityEditor;
using UnityEngine;

namespace Unity.Editor.Bridge
{
    class EntityConversionBridge
    {
        public delegate bool EditorHeaderItemOnGuiCallback(Rect rect, Object[] targets);
        public static event EditorHeaderItemOnGuiCallback editorHeaderItemOnGui;

        [EditorHeaderItem(typeof(Object), int.MaxValue)]
        public static bool EditorHeaderItemOnGui(Rect rect, Object[] targets)
            => editorHeaderItemOnGui?.Invoke(rect, targets) ?? false;
    }
}
