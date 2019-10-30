using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class PostScriptsCallback : IBuildTask
    {
        public int Version { get { return 1; } }

#pragma warning disable 649
        [InjectContext]
        IBuildParameters m_Parameters;

        [InjectContext]
        IBuildResults m_Results;

        [InjectContext(ContextUsage.In)]
        IScriptsCallback m_Callback;
#pragma warning restore 649

        public ReturnCode Run()
        {
            return m_Callback.PostScripts(m_Parameters, m_Results);
        }
    }
}
