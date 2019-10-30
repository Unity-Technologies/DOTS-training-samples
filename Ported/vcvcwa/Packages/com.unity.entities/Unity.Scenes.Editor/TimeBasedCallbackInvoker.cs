using System.Collections.Generic;
using UnityEditor;

namespace Unity.Scenes.Editor
{
    /// <summary>
    /// Invokes registered callbacks during static editor update with a fixed time step.
    /// </summary>
    static class TimeBasedCallbackInvoker
    {
        static List<EditorApplication.CallbackFunction> m_RegisterdCallbacks = new List<EditorApplication.CallbackFunction>();
        const double k_TimeInterval = 0.05f;
        static double m_NextInvocationTime;

        /// <summary>
        /// Register a callback for repeated invocation.
        /// The registration will have no effect if the callback has already been registered.
        /// </summary>
        public static void SetCallback(EditorApplication.CallbackFunction callback)
        {
            if (m_RegisterdCallbacks.Contains(callback))
                return;

            m_RegisterdCallbacks.Add(callback);

            if (m_RegisterdCallbacks.Count == 1)
            {
                EditorApplication.update += InvokeCallbacks;
                m_NextInvocationTime = EditorApplication.timeSinceStartup + k_TimeInterval;
            }
        }

        /// <summary>
        /// Stop repeated invocation of a callback.
        /// </summary>
        public static void ClearCallback(EditorApplication.CallbackFunction callback)
        {
            if (!m_RegisterdCallbacks.Contains(callback))
                return;

            m_RegisterdCallbacks.Remove(callback);

            if (m_RegisterdCallbacks.Count == 0)
                EditorApplication.update -= InvokeCallbacks;
        }

        static void InvokeCallbacks()
        {
            var currentTime = EditorApplication.timeSinceStartup;
            if (currentTime < m_NextInvocationTime)
                return;

            m_NextInvocationTime = currentTime + k_TimeInterval;
            for (var i = m_RegisterdCallbacks.Count - 1; i >= 0 ; i--)
                m_RegisterdCallbacks[i].Invoke();
        }
    }
}