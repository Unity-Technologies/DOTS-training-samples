using System;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Tasks;

namespace UnityEditor.Build.Pipeline.Interfaces
{
    [Serializable]
    public struct CustomContent : IEquatable<CustomContent>
    {
        public GUID Asset { get; set; }
        public Action<GUID, CalculateCustomDependencyData> Processor;

        public bool Equals(CustomContent other)
        {
            return Asset == other.Asset && Processor == other.Processor;
        }
    }

    /// <summary>
    /// Base interface for feeding Assets to the Scriptable Build Pipeline.
    /// </summary>
    public interface IBuildContent : IContextObject
    {
        /// <summary>
        /// List of Assets to include.
        /// </summary>
        List<GUID> Assets { get; }

        /// <summary>
        /// List of Scenes to include.
        /// </summary>
        List<GUID> Scenes { get; }

        List<CustomContent> CustomAssets { get; }
    }

    /// <summary>
    /// Base interface for feeding Assets with explicit Asset Bundle layout to the Scriptable Build Pipeline.
    /// </summary>
    public interface IBundleBuildContent : IBuildContent
    {
        /// <summary>
        /// Specific layout of asset bundles to assets or scenes.
        /// </summary>
        Dictionary<string, List<GUID>> BundleLayout { get; }

        /// <summary>
        /// Additional list of raw files to add to an asset bundle
        /// </summary>
        Dictionary<string, List<ResourceFile>> AddionalFiles { get; }

        Dictionary<GUID, string> FakeAssets { get; }

        /// <summary>
        /// Custom loading identifiers to use for Assets or Scenes.
        /// </summary>
        Dictionary<GUID, string> Addresses { get; }
    }
}