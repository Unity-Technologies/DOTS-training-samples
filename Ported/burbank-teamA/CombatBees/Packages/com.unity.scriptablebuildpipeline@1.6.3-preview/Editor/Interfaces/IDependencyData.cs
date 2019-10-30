using System.Collections.Generic;
using UnityEditor.Build.Content;

namespace UnityEditor.Build.Pipeline.Interfaces
{
    /// <summary>
    /// Base interface for the dependency data container
    /// </summary>
    public interface IDependencyData : IContextObject
    {
        /// <summary>
        /// Map of Asset to dependency data.
        /// </summary>
        Dictionary<GUID, AssetLoadInfo> AssetInfo { get; }

        /// <summary>
        /// Map of Asset to usage data.
        /// </summary>
        Dictionary<GUID, BuildUsageTagSet> AssetUsage { get; }

        /// <summary>
        /// Map of Scene to dependency data.
        /// </summary>
        Dictionary<GUID, SceneDependencyInfo> SceneInfo { get; }

        /// <summary>
        /// Map of Scene to usage data.
        /// </summary>
        Dictionary<GUID, BuildUsageTagSet> SceneUsage { get; }

        /// <summary>
        /// Reusable cache for calculating usage tags
        /// </summary>
        BuildUsageCache DependencyUsageCache { get; }

        /// <summary>
        /// BuildUsageTagGlobal value from GraphicsSettings
        /// </summary>
        BuildUsageTagGlobal GlobalUsage { get; }
    }
}