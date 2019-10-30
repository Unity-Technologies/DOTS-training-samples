using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Utilities;

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class UpdateBundleObjectLayout : IBuildTask
    {
        public int Version { get { return 1; } }

#pragma warning disable 649
        [InjectContext(ContextUsage.In, true)]
        IBundleExplictObjectLayout m_Layout;

        [InjectContext]
        IBundleBuildContent m_Content;

        [InjectContext(ContextUsage.In)]
        IDependencyData m_DependencyData;

        [InjectContext]
        IBundleWriteData m_WriteData;

        [InjectContext(ContextUsage.In)]
        IDeterministicIdentifiers m_PackingMethod;
#pragma warning restore 649

        public ReturnCode Run()
        {
            if (m_Layout == null || m_Layout.ExplicitObjectLocation.IsNullOrEmpty())
                return ReturnCode.SuccessNotRun;

            // Go object by object
            foreach (var pair in m_Layout.ExplicitObjectLocation)
            {
                ObjectIdentifier objectID = pair.Key;
                string bundleName = pair.Value;
                string internalName = string.Format(CommonStrings.AssetBundleNameFormat, m_PackingMethod.GenerateInternalFileName(bundleName));

                // Add dependency on possible new file if asset depends on object
                foreach (KeyValuePair<GUID, AssetLoadInfo> dependencyPair in m_DependencyData.AssetInfo)
                {
                    var asset = dependencyPair.Key;
                    var assetInfo = dependencyPair.Value;
                    var assetFiles = m_WriteData.AssetToFiles[asset];
                    AddFileDependencyIfFound(objectID, internalName, assetFiles, assetInfo.includedObjects);
                    AddFileDependencyIfFound(objectID, internalName, assetFiles, assetInfo.referencedObjects);
                }

                // Add dependency on possible new file if scene depends on object
                foreach (KeyValuePair<GUID, SceneDependencyInfo> dependencyPair in m_DependencyData.SceneInfo)
                {
                    var asset = dependencyPair.Key;
                    var assetInfo = dependencyPair.Value;
                    AddFileDependencyIfFound(objectID, internalName, m_WriteData.AssetToFiles[asset], assetInfo.referencedObjects);
                }

                // Remove object from existing FileToObjects
                foreach (List<ObjectIdentifier> fileObjects in m_WriteData.FileToObjects.Values)
                {
                    if (fileObjects.Contains(objectID))
                        fileObjects.Remove(objectID);
                }

                // Update File to bundle and Bundle layout
                if (!m_WriteData.FileToBundle.ContainsKey(internalName))
                {
                    m_WriteData.FileToBundle.Add(internalName, bundleName);
                    m_Content.BundleLayout.Add(bundleName, new List<GUID>());
                }

                // Update File to object map
                List<ObjectIdentifier> objectIDs;
                m_WriteData.FileToObjects.GetOrAdd(internalName, out objectIDs);
                if (!objectIDs.Contains(objectID))
                    objectIDs.Add(objectID);
            }
            return ReturnCode.Success;
        }

        static void AddFileDependencyIfFound(ObjectIdentifier objectID, string fileName, ICollection<string> assetFiles, ICollection<ObjectIdentifier> collection)
        {
            if (collection.Contains(objectID))
            {
                if (!assetFiles.Contains(fileName))
                    assetFiles.Add(fileName);
            }
        }
    }
}
