using System;
using System.Collections.Generic;
using UnityEditor.Build.Content;

namespace UnityEditor.Build.Pipeline.Interfaces
{
    [Serializable]
    public class ExtendedAssetData
    {
        public List<ObjectIdentifier> Representations { get; set; }

        public ExtendedAssetData()
        {
            Representations = new List<ObjectIdentifier>();
        }
    }

    public interface IBuildExtendedAssetData : IContextObject
    {
        Dictionary<GUID, ExtendedAssetData> ExtendedData { get; }
    }
}
