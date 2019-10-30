using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class SwitchToBuildPlatform : IBuildTask
    {
        public int Version { get { return 1; } }

#pragma warning disable 649
        [InjectContext(ContextUsage.In)]
        IBuildParameters m_Parameters;

        [InjectContext(ContextUsage.In, true)]
        IBuildInterfacesWrapper m_InterfaceWrapper;
#pragma warning restore 649

        public ReturnCode Run()
        {
            if (EditorUserBuildSettings.SwitchActiveBuildTarget(m_Parameters.Group, m_Parameters.Target))
            {
                if (m_InterfaceWrapper != null)
                    m_InterfaceWrapper.InitializeCallbacks();
                return ReturnCode.Success;
            }
            return ReturnCode.Error;
        }
    }
}
