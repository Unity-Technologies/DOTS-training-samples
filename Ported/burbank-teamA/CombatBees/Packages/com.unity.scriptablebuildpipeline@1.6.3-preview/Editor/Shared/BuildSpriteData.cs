using System;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Interfaces;

namespace UnityEditor.Build.Pipeline
{
    [Serializable]
    public class BuildSpriteData : IBuildSpriteData
    {
        public Dictionary<GUID, SpriteImporterData> ImporterData { get; set; }

        public BuildSpriteData()
        {
            ImporterData = new Dictionary<GUID, SpriteImporterData>();
        }
    }
}
