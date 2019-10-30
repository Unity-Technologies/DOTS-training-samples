using System;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UnityEditor.Build.Pipeline
{
    /// <summary>
    /// Basic implementation of IWriteData. Stores the write information calculated during a build.
    /// <seealso cref="IWriteData"/>
    /// </summary>
    [Serializable]
    public class BuildWriteData : IWriteData
    {
        /// <inheritdoc />
        public Dictionary<GUID, List<string>> AssetToFiles { get; private set; }
        /// <inheritdoc />
        public Dictionary<string, List<ObjectIdentifier>> FileToObjects { get; private set; }
        /// <inheritdoc />
        public List<IWriteOperation> WriteOperations { get; private set; }

        /// <summary>
        /// Default constructor, initializes properties to defaults
        /// </summary>
        public BuildWriteData()
        {
            AssetToFiles = new Dictionary<GUID, List<string>>();
            FileToObjects = new Dictionary<string, List<ObjectIdentifier>>();
            WriteOperations = new List<IWriteOperation>();
        }
    }

    /// <summary>
    /// Basic implementation of IBundleWriteData. Stores the asset bundle write information calculated during a build.
    /// <seealso cref="IBundleWriteData"/>
    /// </summary>
    [Serializable]
    public class BundleWriteData : IBundleWriteData
    {
        public Dictionary<GUID, List<string>> AssetToFiles { get; private set; }
        /// <inheritdoc />
        public Dictionary<string, List<ObjectIdentifier>> FileToObjects { get; private set; }
        /// <inheritdoc />
        public Dictionary<string, string> FileToBundle { get; private set; }
        /// <inheritdoc />
        public Dictionary<string, BuildUsageTagSet> FileToUsageSet { get; private set; }
        /// <inheritdoc />
        public Dictionary<string, BuildReferenceMap> FileToReferenceMap { get; private set; }
        /// <inheritdoc />
        public List<IWriteOperation> WriteOperations { get; private set; }

        /// <summary>
        /// Default constructor, initializes properties to defaults
        /// </summary>
        public BundleWriteData()
        {
            AssetToFiles = new Dictionary<GUID, List<string>>();
            FileToObjects = new Dictionary<string, List<ObjectIdentifier>>();
            FileToBundle = new Dictionary<string, string>();
            FileToUsageSet = new Dictionary<string, BuildUsageTagSet>();
            FileToReferenceMap = new Dictionary<string, BuildReferenceMap>();
            WriteOperations = new List<IWriteOperation>();
        }

    }
}
