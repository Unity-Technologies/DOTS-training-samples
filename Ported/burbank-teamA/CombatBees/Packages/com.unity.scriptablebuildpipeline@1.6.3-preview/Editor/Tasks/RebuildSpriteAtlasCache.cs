using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Sprites;
using UnityEditor.U2D;

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class RebuildSpriteAtlasCache : IBuildTask
    {
        public int Version { get { return 1; } }

#pragma warning disable 649
        [InjectContext(ContextUsage.In)]
        IBuildParameters m_Parameters;
#pragma warning restore 649

        public ReturnCode Run()
        {
            // TODO: Need a return value if this ever can fail
            Packer.RebuildAtlasCacheIfNeeded(m_Parameters.Target, true, Packer.Execution.Normal);
			SpriteAtlasUtility.PackAllAtlases(m_Parameters.Target);
            return ReturnCode.Success;
        }
    }
}
