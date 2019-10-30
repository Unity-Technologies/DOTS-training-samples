using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Player;

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class BuildPlayerScripts : IBuildTask
    {
        public int Version { get { return 1; } }

#pragma warning disable 649
        [InjectContext]
        IBuildParameters m_Parameters;

        [InjectContext]
        IBuildResults m_Results;
#pragma warning restore 649

        public ReturnCode Run()
        {
            if (m_Parameters.ScriptInfo != null)
                return ReturnCode.SuccessNotRun;

            // TODO: Replace with call to GetTempOrCachePath
            // TODO: Create tasks to copy scripts to correct output folder?
            m_Results.ScriptResults = PlayerBuildInterface.CompilePlayerScripts(m_Parameters.GetScriptCompilationSettings(), m_Parameters.TempOutputFolder);
            m_Parameters.ScriptInfo = m_Results.ScriptResults.typeDB;

            if (m_Results.ScriptResults.assemblies.IsNullOrEmpty() && m_Results.ScriptResults.typeDB == null)
                return ReturnCode.Error;
            return ReturnCode.Success;
        }
    }
}
