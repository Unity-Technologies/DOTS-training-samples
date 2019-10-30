using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class PostDependencyCallback : IBuildTask
    {
        public int Version { get { return 1; } }

#pragma warning disable 649
        [InjectContext]
        IBuildParameters m_Parameters;

        [InjectContext]
        IDependencyData m_DependencyData;

        [InjectContext(ContextUsage.In)]
        IDependencyCallback m_Callback;
#pragma warning restore 649

        public ReturnCode Run()
        {
            return m_Callback.PostDependency(m_Parameters, m_DependencyData);
        }
    }
}
