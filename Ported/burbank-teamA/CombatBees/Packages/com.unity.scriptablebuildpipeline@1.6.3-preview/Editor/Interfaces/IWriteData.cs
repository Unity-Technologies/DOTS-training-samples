using System.Collections.Generic;
using UnityEditor.Build.Content;

namespace UnityEditor.Build.Pipeline.Interfaces
{
    /// <summary>
    /// Base interface for the write data container.
    /// </summary>
    public interface IWriteData : IContextObject
    {
        /// <summary>
        /// Map of asset to file dependencies.
        /// First dependency in the list is the main file for an asset.
        /// </summary>
        Dictionary<GUID, List<string>> AssetToFiles { get; }

        /// <summary>
        /// Map of file to list of objects in that file
        /// </summary>
        Dictionary<string, List<ObjectIdentifier>> FileToObjects { get; }

        /// <summary>
        /// List of all write operations to serialize data to disk
        /// </summary>
        List<IWriteOperation> WriteOperations { get; }
    }

    /// <summary>
    /// Extended interface for Asset Bundle write data container.
    /// </summary>
    public interface IBundleWriteData : IWriteData
    {
        /// <summary>
        /// Map of file name to bundle name
        /// </summary>
        Dictionary<string, string> FileToBundle { get; }

        /// <summary>
        /// Map of file name to calculated usage set
        /// </summary>
        Dictionary<string, BuildUsageTagSet> FileToUsageSet { get; }

        /// <summary>
        /// Map of file name to calculated object references
        /// </summary>
        Dictionary<string, BuildReferenceMap> FileToReferenceMap { get; }
    }
}