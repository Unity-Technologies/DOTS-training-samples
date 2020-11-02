namespace Unity.Editor.Bridge
{
#if UNITY_EDITOR
    static class GUIUtility
    {
        public static double pixelsPerPoint => UnityEngine.GUIUtility.pixelsPerPoint;
    }
#endif
}
