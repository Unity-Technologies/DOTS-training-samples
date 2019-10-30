using System;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.SceneManagement;
using System.Linq;

namespace UnityEditor.Build.Utilities
{
    public class SceneStateCleanup : IDisposable
    {
        SceneSetup[] m_Scenes;

        bool m_Disposed;

        public SceneStateCleanup()
        {
            m_Scenes = EditorSceneManager.GetSceneManagerSetup();
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

            m_Disposed = true;

            if (disposing)
            {
                // Test runner injects scenes, so we strip those here
                var scenes = m_Scenes.Where(s => !string.IsNullOrEmpty(s.path)).ToArray();
                if (!scenes.IsNullOrEmpty())
                    EditorSceneManager.RestoreSceneManagerSetup(scenes);
                else
                    EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            }
        }
    }
}
