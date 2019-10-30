using System;
using Unity.Mathematics;

namespace Unity.Entities
{
    [Serializable]
    public struct SceneSectionData : IComponentData
    {
        public Hash128          SceneGUID;
        public int              SubSectionIndex;
        public int              FileSize;
        public int              ObjectReferenceCount;
        public MinMaxAABB       BoundingVolume;

        [Obsolete("SharedComponentCount from the deprecated SceneData API is obsolete and will be (RemovedAfter 2019-10-6)", false)]
        public int SharedComponentCount
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }

    public struct SceneReference : IComponentData, IEquatable<SceneReference>
    {
        public Hash128 SceneGUID;

        public bool Equals(SceneReference other)
        {
            return SceneGUID.Equals(other.SceneGUID);
        }
        public override int GetHashCode()
        {
            return SceneGUID.GetHashCode();
        }
    }

    [System.Serializable]
    public struct SceneSection : ISharedComponentData, IEquatable<SceneSection>
    {
        public Hash128        SceneGUID;
        public int            Section;

        public bool Equals(SceneSection other)
        {
            return SceneGUID.Equals(other.SceneGUID) && Section == other.Section;
        }

        public override int GetHashCode()
        {
            return (SceneGUID.GetHashCode() * 397) ^ Section;
        }
    }

    [Flags]
    public enum SceneLoadFlags
    {
        AutoLoad = 1,
        BlockOnImport = 2
    }
    
    public struct RequestSceneLoaded : IComponentData
    {
        public SceneLoadFlags LoadFlags;
    }
}
