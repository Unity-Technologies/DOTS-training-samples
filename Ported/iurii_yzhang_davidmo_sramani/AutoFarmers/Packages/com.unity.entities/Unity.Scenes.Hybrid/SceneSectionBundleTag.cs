using System;
using Unity.Entities;

namespace Unity.Scenes
{
    [Serializable]
    public struct SceneSectionBundle : ISharedComponentData, IEquatable<SceneSectionBundle>, IRefCounted
    {
        private int RefCount;
        public UnityEngine.AssetBundle Bundle;

        public SceneSectionBundle(UnityEngine.AssetBundle bundle)
        {
            Bundle = bundle;
            RefCount = 0;
        }

        public void Release()
        {
            RefCount--;
            if (RefCount <= 0 && Bundle)
            {
                Bundle.Unload(true);
                Bundle = null;
            }
        }

        public void Retain()
        {
            RefCount++;
        }

        public bool Equals(SceneSectionBundle other)
        {
            return Bundle == other.Bundle;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            if (!ReferenceEquals(Bundle, null)) hash ^= Bundle.GetHashCode();
            return hash;
        }
    }
}