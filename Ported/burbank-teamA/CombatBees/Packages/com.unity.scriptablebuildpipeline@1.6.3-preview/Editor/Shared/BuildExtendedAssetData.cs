using System;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UnityEditor.Build.Pipeline
{
    [Serializable]
    public class BuildExtendedAssetData : IBuildExtendedAssetData
    {
        public Dictionary<GUID, ExtendedAssetData> ExtendedData { get; private set; }

        public BuildExtendedAssetData()
        {
            ExtendedData = new Dictionary<GUID, ExtendedAssetData>();
        }
    }
}
