using Unity.Platforms;

namespace Unity.Build
{
    public interface IBuildProfileComponent : IBuildSettingsComponent
    {
        BuildTarget GetBuildTarget();
        BuildPipeline GetBuildPipeline();
    }
}
