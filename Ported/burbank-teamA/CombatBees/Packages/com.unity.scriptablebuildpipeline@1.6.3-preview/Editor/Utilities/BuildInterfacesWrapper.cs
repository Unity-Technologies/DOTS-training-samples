using System;
using System.Reflection;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UnityEditor.Build.Pipeline.Utilities
{
    public interface IBuildInterfacesWrapper : IContextObject
    {
        void InitializeCallbacks();
        void CleanupCallbacks();
    }

    public class BuildInterfacesWrapper : IDisposable, IBuildInterfacesWrapper
    {
        Type m_Type = null;

        bool m_Disposed = false;

        public BuildInterfacesWrapper()
        {
            m_Type = Type.GetType("UnityEditor.Build.BuildPipelineInterfaces, UnityEditor");
            InitializeCallbacks();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (m_Disposed)
                return;

            CleanupCallbacks();
            m_Disposed = true;
        }

        public void InitializeCallbacks()
        {
            var init = m_Type.GetMethod("InitializeBuildCallbacks", BindingFlags.NonPublic | BindingFlags.Static);
            init.Invoke(null, new object[] { 18 }); // 18 = BuildCallbacks.SceneProcessors | BuildCallbacks.ShaderProcessors
        }

        public void CleanupCallbacks()
        {
            var clean = m_Type.GetMethod("CleanupBuildCallbacks", BindingFlags.NonPublic | BindingFlags.Static);
            clean.Invoke(null, null);
        }
    }
}
