#if !NET_DOTS
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

[assembly: InternalsVisibleTo("Unity.Scenes.Editor")]
namespace Unity.Entities.Serialization
{
    internal class GlobalAssetObjectResolver
    {
        struct Resolved
        {
            public AssetObjectManifest AssetObjectManifest;
            public AssetBundle AssetBundle;
            public Hash128 TargetHash;
        }

        private Dictionary<Hash128, Resolved> _Assets = new Dictionary<Hash128, Resolved>();

        public void AddAsset(Hash128 guid, Hash128 targetHash, AssetObjectManifest manifest, AssetBundle assetBundle)
        {
            _Assets[guid] = new Resolved
            {
                AssetObjectManifest = manifest,
                AssetBundle = assetBundle,
                TargetHash = targetHash
            };
        }

        public void UpdateTargetHash(Hash128 guid, Hash128 targetHash)
        {
            var resolved = _Assets[guid];
            resolved.TargetHash = targetHash;
            _Assets[guid] = resolved;
        }


        public void UpdateObjectManifest(Hash128 guid, AssetObjectManifest manifest)
        {
            var resolved = _Assets[guid];
            resolved.AssetObjectManifest = manifest;
            _Assets[guid] = resolved;
        }


        public void Validate(Hash128 guid)
        {
            if (!_Assets.TryGetValue(guid, out var resolved))
            {
                Debug.LogError("GUID not known");
                return;
            }

            if (resolved.AssetBundle == null)
            {
                Debug.LogError($"AssetBundle '{guid.ToString()}' not loadable");
                return;
            }

            if (resolved.AssetObjectManifest == null)
            {
                Debug.LogError($"ObjectManifest in '{guid.ToString()}' not loadable");
                return;
            }

//@TODO: We currently have no way of stripping objects that are editor only,
// so we can't perform these checks because some of these objects will in fact be null
#if false
            for (int i = 0; i < resolved.AssetObjectManifest.Objects.Length; i++)
            {
                Object obj = resolved.AssetObjectManifest.Objects[i];
                if (obj == null)
                {
                    //@TODO: Follow up with ryan why these are failing
                    if (guid.ToString() != k_BuiltInResourcesGuid)
                        Debug.LogError($"Object in '{guid.ToString()}' not loadable at index {i}.");
                }
            }
#endif
        }

        public bool HasAsset(Hash128 hash)
        {
            return _Assets.ContainsKey(hash);
        }

        public UnityEngine.Object ResolveObject(RuntimeGlobalObjectId objID)
        {
            if (!_Assets.TryGetValue(objID.AssetGUID, out var manifest))
                return null;

            //@TODO-PERF: sort by GlobalObjectIDs and do binary search to find the right object  
            var objectIDs = manifest.AssetObjectManifest.GlobalObjectIds;
            for (int i = 0; i != objectIDs.Length; i++)
            {
                if (objectIDs[i].Equals(objID))
                    return manifest.AssetObjectManifest.Objects[i];
            }

            return null;
        }

        unsafe public void ResolveObjects(NativeArray<RuntimeGlobalObjectId> globalObjectIDs, Object[] objects)
        {
            var globalObjectIDsPtr = (RuntimeGlobalObjectId*)globalObjectIDs.GetUnsafePtr();
            for (int i = 0; i != globalObjectIDs.Length; i++)
                objects[i] = ResolveObject(globalObjectIDsPtr[i]);
        }

        public AssetBundle GetAssetBundle(Hash128 requestedGUID)
        {
            if (_Assets.TryGetValue(requestedGUID, out var resolved))
                return resolved.AssetBundle;
            else
                return null;
        }

        public void DisposeObjectManifests()
        {
            foreach (var resolved in _Assets.Values)
                UnityEngine.Object.DestroyImmediate(resolved.AssetObjectManifest);
            _Assets = null;
        }

        public void DisposeAssetBundles()
        {
            foreach (var resolved in _Assets.Values)
                resolved.AssetBundle.Unload(true);
            _Assets = null;
        }
    }
}
#endif