using System.IO;
using Unity.Build;
using Unity.Properties;

namespace Unity.Build.Common
{
    /// <summary>
    /// Overrides the default output directory of Builds/NameOfBuildSettingsAsset to an arbitrary location. 
    /// </summary>
    public class OutputBuildDirectory : IBuildSettingsComponent
    {
        [Property]
        public string OutputDirectory { get; set; }

        public static string GetOutputDirectoryFor(BuildContext context)
        {
            context.BuildSettings.TryGetComponent<OutputBuildDirectory>(out var outDirComponent);
            return outDirComponent?.OutputDirectory ?? $"build/{context.BuildSettings.name}";
        }
    }
}