using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Content;
using UnityEditor.Build.Pipeline.Injector;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEditor.Build.Pipeline.WriteTypes;

namespace UnityEditor.Build.Pipeline.Tasks
{
    public class GenerateSubAssetPathMaps : IBuildTask
    {
        public int Version { get { return 1; } }

#pragma warning disable 649
        [InjectContext]
        IBundleWriteData m_WriteData;

        [InjectContext(ContextUsage.In, true)]
        IBuildExtendedAssetData m_ExtendedAssetData;
#pragma warning restore 649

        public ReturnCode Run()
        {
            if (m_ExtendedAssetData == null || m_ExtendedAssetData.ExtendedData.IsNullOrEmpty())
                return ReturnCode.SuccessNotRun;

            Dictionary<string, IWriteOperation> fileToOperation = m_WriteData.WriteOperations.ToDictionary(x => x.Command.internalName, x => x);
            foreach (var pair in m_ExtendedAssetData.ExtendedData)
            {
                GUID asset = pair.Key;
                string mainFile = m_WriteData.AssetToFiles[asset][0];
                var abOp = fileToOperation[mainFile] as AssetBundleWriteOperation;

                int assetInfoIndex = abOp.Info.bundleAssets.FindIndex(x => x.asset == asset);
                AssetLoadInfo assetInfo = abOp.Info.bundleAssets[assetInfoIndex];
                int offset = 1;
                foreach (var subAsset in pair.Value.Representations)
                {
                    var secondaryAssetInfo = CreateSubAssetLoadInfo(assetInfo, subAsset);
                    abOp.Info.bundleAssets.Insert(assetInfoIndex + offset, secondaryAssetInfo);
                    offset++;
                }
            }

            return ReturnCode.Success;
        }

        static AssetLoadInfo CreateSubAssetLoadInfo(AssetLoadInfo assetInfo, ObjectIdentifier subAsset)
        {
            var subAssetLoadInfo = new AssetLoadInfo();
            subAssetLoadInfo.asset = assetInfo.asset;
            subAssetLoadInfo.address = assetInfo.address;
            subAssetLoadInfo.referencedObjects = new List<ObjectIdentifier>(assetInfo.referencedObjects);
            subAssetLoadInfo.includedObjects = new List<ObjectIdentifier>(assetInfo.includedObjects);

            var index = subAssetLoadInfo.includedObjects.IndexOf(subAsset);
            subAssetLoadInfo.includedObjects.Swap(0, index);
            return subAssetLoadInfo;
        }
    }
}
