using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEditor.Build.Player;
using UnityEngine.Build.Pipeline;

namespace UnityEditor.Build.Pipeline.Interfaces
{
    /// <summary>
    /// Base interface for the build results container
    /// </summary>
    public interface IBuildResults : IContextObject
    {
        /// <summary>
        /// Results from the script compiling step.
        /// </summary>
        ScriptCompilationResult ScriptResults { get; set; }

        /// <summary>
        /// Map of serialized file name to results for built content.
        /// </summary>
        Dictionary<string, WriteResult> WriteResults { get; }
    }

    /// <summary>
    /// Extended interface for Asset Bundle build results container.
    /// <seealso cref="IBuildResults"/>
    /// </summary>
    public interface IBundleBuildResults : IBuildResults
    {
        /// <summary>
        /// Map of Asset Bundle name to details about the built bundle.
        /// </summary>
        Dictionary<string, BundleDetails> BundleInfos { get; }
    }
}