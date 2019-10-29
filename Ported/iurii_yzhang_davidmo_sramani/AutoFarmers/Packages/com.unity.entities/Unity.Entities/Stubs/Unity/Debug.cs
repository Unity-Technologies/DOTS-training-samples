using System;
#if !NET_DOTS
using UnityObject = UnityEngine.Object;
#endif

namespace Unity
{
    // TODO: provide an implementation of Unity.Debug that does not rely on UnityEngine and modernizes this API
    // (for now it's just here for easier compatibility and fwd migration)
    static class Debug
    {
        public static void LogError(object message) =>
            UnityEngine.Debug.LogError(message);
        public static void LogWarning(string message) =>
            UnityEngine.Debug.LogWarning(message);
        public static void Log(string message) =>
            UnityEngine.Debug.Log(message);
        public static void LogException(Exception exception) =>
            UnityEngine.Debug.LogException(exception);

        #if !NET_DOTS
        public static void LogError(object message, UnityObject context) =>
            UnityEngine.Debug.LogError(message, context);
        public static void LogWarning(string message, UnityObject context) =>
            UnityEngine.Debug.LogWarning(message, context);
        public static void Log(string message, UnityObject context) =>
            UnityEngine.Debug.Log(message, context);
        public static void LogException(Exception exception, UnityObject context) =>
            UnityEngine.Debug.LogException(exception, context);
        #endif
    }
}
