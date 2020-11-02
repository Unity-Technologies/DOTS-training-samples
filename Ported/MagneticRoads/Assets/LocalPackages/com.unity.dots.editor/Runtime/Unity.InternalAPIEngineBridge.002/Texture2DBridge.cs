using UnityEngine;

namespace Unity.Editor.Bridge
{
#if UNITY_EDITOR
    static class Texture2DExtensions
    {
        public static float GetPixelsPerPoint(this Texture2D texture2D)
        {
            return texture2D.pixelsPerPoint;
        }
    }
#endif
}
