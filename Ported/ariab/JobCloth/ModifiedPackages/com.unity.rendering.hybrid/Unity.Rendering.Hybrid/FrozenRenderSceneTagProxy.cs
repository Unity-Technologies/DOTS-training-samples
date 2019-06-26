using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Rendering
{
    [Serializable]
    public struct FrozenRenderSceneTag : ISharedComponentData, IEquatable<FrozenRenderSceneTag>
    {
        public Hash128          SceneGUID;
        public int              SectionIndex;
        public int              HasStreamedLOD;

        public bool Equals(FrozenRenderSceneTag other)
        {
            return SceneGUID == other.SceneGUID && SectionIndex == other.SectionIndex;
        }

        public override int GetHashCode()
        {
            return SceneGUID.GetHashCode() ^ SectionIndex;
        }
    }

    [UnityEngine.AddComponentMenu("Hidden/DontUse")]
    public class FrozenRenderSceneTagProxy : SharedComponentDataProxy<FrozenRenderSceneTag>
    {
    }
}
