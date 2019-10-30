using UnityEditor.Build.Content;
using UnityEditor.Build.Player;
using UnityEngine;

namespace UnityEditor.Build.Pipeline.Interfaces
{
#if UNITY_2018_3_OR_NEWER
    using BuildCompression = UnityEngine.BuildCompression;
#else
    using BuildCompression = UnityEditor.Build.Content.BuildCompression;
#endif

    /// <summary>
    /// Base interface for the parameters container
    /// </summary>
    public interface IBuildParameters : IContextObject
    {
        /// <summary>
        /// Target build platform. <seealso cref="BuildTarget"/>
        /// </summary>
        BuildTarget Target { get; set; }

        /// <summary>
        /// Target build platform group. <seealso cref="BuildTargetGroup"/>
        /// </summary>
        BuildTargetGroup Group { get; set; }

        /// <summary>
        /// The set of build flags to use for building content.
        /// </summary>
        ContentBuildFlags ContentBuildFlags { get; set; }

        /// <summary>
        /// Scripting type information to use when building content.
        /// Setting this to a previously cached value will prevent the default script compiling step.
        /// </summary>
        TypeDB ScriptInfo { get; set; }

        /// <summary>
        /// Script compilation options to use for the script compiling step.
        /// </summary>
        ScriptCompilationOptions ScriptOptions { get; set; }

        /// <summary>
        /// Temporary location to be used for artifacts generated during the build but are not part of the final output.
        /// </summary>
        string TempOutputFolder { get; set; }

        /// <summary>
        /// Enables the use of the build cache if set to true.
        /// </summary>
        bool UseCache { get; set; }

        /// <summary>
        /// Enables & specifies the cache server to use.
        /// </summary>
        string CacheServerHost { get; set; }

        /// <summary>
        /// The port for the cache server to use
        /// </summary>
        int CacheServerPort { get; set; }

        /// <summary>
        /// Constructs and returns the BuildSettings struct to use for content building.
        /// </summary>
        /// <returns>Returns the BuildSettings struct to use for content building.</returns>
        BuildSettings GetContentBuildSettings();

        /// <summary>
        /// Returns the output folder to use for the specified identifier.
        /// </summary>
        /// <param name="identifier">Identifier used to identify which output folder to use.</param>
        /// <returns>Returns the output folder to use for the specified identifier.</returns>
        string GetOutputFilePathForIdentifier(string identifier);

        /// <summary>
        /// Constructs and returns the BuildCompression struct to use for the specified identifier.
        /// </summary>
        /// <param name="identifier">Identifier used to construct the BuildCompression struct.</param>
        /// <returns>Returns the BuildCompression struct to use for a specific identifier.</returns>
        BuildCompression GetCompressionForIdentifier(string identifier);

        /// <summary>
        /// Constructs and returns the ScriptCompilationSettings struct to use for script compiling.
        /// </summary>
        /// <returns>Returns the ScriptCompilationSettings struct to use for script compiling.</returns>
        ScriptCompilationSettings GetScriptCompilationSettings();
    }

    public interface IBundleBuildParameters : IBuildParameters
    {
        /// <summary>
        /// Append the hash to the assetBundle file name.
        /// </summary>
        bool AppendHash { get; set; }
    }
}
