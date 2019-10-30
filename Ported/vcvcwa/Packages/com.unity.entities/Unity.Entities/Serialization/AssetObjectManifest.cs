#if !NET_DOTS
using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: InternalsVisibleTo("Unity.Scenes.Editor")]
namespace Unity.Entities.Serialization
{
    internal class AssetObjectManifest : ScriptableObject
    {
        public RuntimeGlobalObjectId[] GlobalObjectIds;
        public Object[]                Objects;
    }

    #if UNITY_EDITOR
    internal class AssetObjectManifestBuilder
    {
        unsafe static public void BuildManifest(GUID guid, AssetObjectManifest manifest)
        {
            var objects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(guid.ToString()));
            BuildManifest(objects, manifest);
        }
        
        unsafe static public void BuildManifest(Object[] objects, AssetObjectManifest manifest)
        {
            manifest.Objects = objects;
            manifest.GlobalObjectIds = new RuntimeGlobalObjectId[objects.Length];
            var globalobjectIds = new GlobalObjectId[objects.Length];

            GlobalObjectId.GetGlobalObjectIdsSlow(objects, globalobjectIds);

            fixed (GlobalObjectId* src = globalobjectIds)
            fixed (RuntimeGlobalObjectId* dst = manifest.GlobalObjectIds)
            {
                UnsafeUtility.MemCpy(dst, src, UnsafeUtility.SizeOf<RuntimeGlobalObjectId>() * objects.Length);
            }
        }
    }
    #endif

    [System.Serializable]
    internal struct RuntimeGlobalObjectId : IEquatable<RuntimeGlobalObjectId>
    {
        public ulong   SceneObjectIdentifier0;
        public ulong   SceneObjectIdentifier1;
        public Hash128 AssetGUID;
        public int     IdentifierType;

        public bool Equals(RuntimeGlobalObjectId other)
        {
            return SceneObjectIdentifier0 == other.SceneObjectIdentifier0 && SceneObjectIdentifier1 == other.SceneObjectIdentifier1 && AssetGUID.Equals(other.AssetGUID) && IdentifierType == other.IdentifierType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = SceneObjectIdentifier0.GetHashCode();
                hashCode = (hashCode * 397) ^ SceneObjectIdentifier1.GetHashCode();
                hashCode = (hashCode * 397) ^ AssetGUID.GetHashCode();
                hashCode = (hashCode * 397) ^ IdentifierType;
                return hashCode;
            }
        }
    }
}
#endif