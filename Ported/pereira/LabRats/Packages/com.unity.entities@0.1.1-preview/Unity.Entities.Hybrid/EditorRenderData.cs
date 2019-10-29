using System;
using UnityEngine;

namespace Unity.Entities
{
    public struct EditorRenderData : ISharedComponentData, IEquatable<EditorRenderData>
    {
        public const UInt64 LiveLinkEditSceneViewMask = 1UL << 60;
        public const UInt64 LiveLinkEditGameViewMask = 1UL << 59;
        
        public ulong      SceneCullingMask;
        public GameObject PickableObject;

        public bool Equals(EditorRenderData other)
        {
            return
                SceneCullingMask == other.SceneCullingMask &&
                PickableObject == other.PickableObject;
        }

        public override int GetHashCode()
        {
            int hash = SceneCullingMask.GetHashCode();

            if (!ReferenceEquals(PickableObject, null))
                hash ^= PickableObject.GetHashCode();

            return hash;
        }
    }
}
