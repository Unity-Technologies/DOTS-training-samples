using System;
using System.Collections.Generic;
using UnityEditor.Build.Content;

namespace UnityEditor.Build.Pipeline.Interfaces
{
    [Serializable]
    public class SpriteImporterData
    {
        public bool PackedSprite { get; set; }
        public ObjectIdentifier SourceTexture { get; set; }
    }

    public interface IBuildSpriteData : IContextObject
    {
        Dictionary<GUID, SpriteImporterData> ImporterData { get; }
    }
}
