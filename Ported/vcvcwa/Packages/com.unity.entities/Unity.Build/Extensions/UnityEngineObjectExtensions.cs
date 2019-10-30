using System.Collections.Generic;

namespace Unity.Build
{
    internal static class UnityEngineObjectExtensions
    {
        static readonly HashSet<UnityEngine.Object> s_PinnedObjects;

        static UnityEngineObjectExtensions()
        {
            s_PinnedObjects = new HashSet<UnityEngine.Object>();
        }

        public static void GCPin(this UnityEngine.Object obj)
        {
            s_PinnedObjects.Add(obj);
        }

        public static void GCUnPin(this UnityEngine.Object obj)
        {
            s_PinnedObjects.Remove(obj);
        }
    }
}
