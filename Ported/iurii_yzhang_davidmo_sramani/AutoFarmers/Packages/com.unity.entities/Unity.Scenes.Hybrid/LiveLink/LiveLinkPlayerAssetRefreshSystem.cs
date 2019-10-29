using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;
using UnityEngine.Experimental.AssetBundlePatching;
using UnityEngine.Networking.PlayerConnection;
using Hash128 = Unity.Entities.Hash128;

namespace Unity.Scenes
{
    struct ResolvedAssetID : IEquatable<ResolvedAssetID>
    {
        public Hash128 GUID;
        public Hash128 TargetHash;

        public bool Equals(ResolvedAssetID other)
        {
            return GUID == other.GUID && TargetHash == other.TargetHash;
        }
    }


#if UNITY_EDITOR
    [DisableAutoCreation]
#endif
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(LiveLinkRuntimeSystemGroup))]
    class LiveLinkPlayerAssetRefreshSystem : ComponentSystem
    {
        public static GlobalAssetObjectResolver _GlobalAssetObjectResolver = new GlobalAssetObjectResolver();

        private Dictionary<Hash128, Hash128>    _WaitingForAssets = new Dictionary<Hash128, Hash128>();

        private EntityQuery                     _ResourceRequests;

        // The resource has been requested from the editor but not necessarily been loaded yet.
        public struct ResourceRequested : IComponentData {}

        public static GlobalAssetObjectResolver GlobalAssetObjectResolver => _GlobalAssetObjectResolver;


        protected override void OnStartRunning()
        {
            PlayerConnection.instance.Register(LiveLinkMsg.ResponseAssetBundleForGUID, ReceiveAssetBundle);
            PlayerConnection.instance.Register(LiveLinkMsg.ResponseAssetBundleTargetHash, ReceiveResponseAssetBundleTargetHash);

            _ResourceRequests = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadOnly<ResourceGUID>() },
                None = new[] { ComponentType.ReadOnly<ResourceRequested>(), ComponentType.ReadOnly<ResourceLoaded>() }
            });
        }

        protected override void OnStopRunning()
        {
            PlayerConnection.instance.Unregister(LiveLinkMsg.ResponseAssetBundleForGUID, ReceiveAssetBundle);
            PlayerConnection.instance.Unregister(LiveLinkMsg.ResponseAssetBundleTargetHash, ReceiveResponseAssetBundleTargetHash);
        }

        string GetCachePath(Hash128 targetHash)
        {
            return $"{Application.persistentDataPath}/{targetHash}";
        }

        string GetTempCachePath()
        {
            return $"{Application.persistentDataPath}/{Path.GetRandomFileName()}";
        }

        //@TODO: Support some sort of transaction like API so we can reload all changed things in batch.
        unsafe void ReceiveAssetBundle(MessageEventArgs args)
        {
            LiveLinkMsg.LogReceived($"AssetBundle: '{args.data.Length}' bytes");

            fixed (byte* ptr = args.data)
            {
                var reader = new UnsafeAppendBuffer.Reader(ptr, args.data.Length);
                var asset = reader.ReadNext<ResolvedAssetID>();

                var assetBundleCachePath = GetCachePath(asset.TargetHash);
                var tempCachePath = GetTempCachePath();

                // cache: look up asset by target hash to see if the version we want is already on the target device

                //if we already have the asset bundle revision we want, then just put that in the resolver as the active revision of the asset
                // cache: if not in cache, write actual file to Application.persistentDatapath
                var stream = File.OpenWrite(tempCachePath);
                stream.Write(args.data, reader.Offset, args.data.Length - reader.Offset);
                stream.Close();
                stream.Dispose();

                LiveLinkMsg.LogInfo($"ReceiveAssetBundle => {asset.GUID} | {asset.TargetHash}, '{tempCachePath}' => '{assetBundleCachePath}'");

                if (File.Exists(assetBundleCachePath))
                {
                    Debug.LogError($"Received {asset.GUID} | {asset.TargetHash} but it already exists on disk");
                    LiveLinkMsg.LogInfo($"Received {asset.GUID} | {asset.TargetHash} but it already exists on disk");
                    File.Delete(tempCachePath);
                }
                else
                {
                    try
                    {
                        File.Move(tempCachePath, assetBundleCachePath);
                    }
                    catch (Exception e)
                    {
                        File.Delete(tempCachePath);
                        if (!File.Exists(assetBundleCachePath))
                        {
                            Debug.LogError($"Failed to move temporary file. Exception: {e.Message}");
                            LiveLinkMsg.LogInfo($"Failed to move temporary file. Exception: {e.Message}");
                        }
                    }
                }
                    

                if (!_WaitingForAssets.ContainsKey(asset.GUID))
                {
                    Debug.LogError($"Received {asset.GUID} | {asset.TargetHash} without requesting it");
                    LiveLinkMsg.LogInfo($"Received {asset.GUID} | {asset.TargetHash} without requesting it");
                }

                _WaitingForAssets[asset.GUID] = asset.TargetHash;
            }
        }

        void LoadAssetBundles(NativeArray<ResolvedAssetID> assets)
        {
            LiveLinkMsg.LogInfo("--- Begin Load asset bundles");

            var patchAssetBundles = new List<AssetBundle>();
            var patchAssetBundlesPath = new List<string>();
            var newAssetBundles = new List<Hash128>();
            var assetBundleToValidate = new List<Hash128>();


            foreach (var asset in assets)
            {
                var assetGUID = asset.GUID;
                var targetHash = asset.TargetHash;
                var assetBundleCachePath = GetCachePath(targetHash);

                //if we already loaded an asset bundle and we just need a refresh
                var oldAssetBundle = _GlobalAssetObjectResolver.GetAssetBundle(assetGUID);
                if (oldAssetBundle != null)
                {
                    LiveLinkMsg.LogInfo($"patching asset bundle: {assetGUID}");

                    patchAssetBundles.Add(oldAssetBundle);
                    patchAssetBundlesPath.Add(assetBundleCachePath);

                    _GlobalAssetObjectResolver.UpdateTargetHash(assetGUID, targetHash);
                }
                else
                {
                    LiveLinkMsg.LogInfo($"Loaded asset bundle: {assetGUID}");

                    var loadedAssetBundle = AssetBundle.LoadFromFile(assetBundleCachePath);
                    _GlobalAssetObjectResolver.AddAsset(assetGUID, targetHash, null, loadedAssetBundle);
                    newAssetBundles.Add(assetGUID);
                }

                assetBundleToValidate.Add(assetGUID);

                //@TODO: Keep a hashtable of guid -> entity?
                Entities.ForEach((Entity entity, ref ResourceGUID guid) =>
                {
                    if (guid.Guid == assetGUID)
                        EntityManager.AddComponentData(entity, new ResourceLoaded());
                });
            }


            AssetBundleUtility.PatchAssetBundles(patchAssetBundles.ToArray(), patchAssetBundlesPath.ToArray());

            foreach (var assetGUID in newAssetBundles)
            {
                var assetBundle = _GlobalAssetObjectResolver.GetAssetBundle(assetGUID);
                if (assetBundle == null)
                {
                    Debug.LogError($"Could not load requested asset bundle.'");
                    return;
                }

                var loadedManifest = assetBundle.LoadAsset<AssetObjectManifest>(assetGUID.ToString());
                if (loadedManifest == null)
                {
                    Debug.LogError($"Loaded {assetGUID} failed to load ObjectManifest");
                    return;
                }

                _GlobalAssetObjectResolver.UpdateObjectManifest(assetGUID, loadedManifest);
            }

            foreach(var assetGUID in assetBundleToValidate)
                _GlobalAssetObjectResolver.Validate(assetGUID);

            LiveLinkMsg.LogInfo("--- End Load asset bundles");
        }

        unsafe void ReceiveResponseAssetBundleTargetHash(MessageEventArgs args)
        {
            using (var resolvedAssets = args.ReceiveArray<ResolvedAssetID>())
            {
                foreach(var asset in resolvedAssets)
                {
                    //TODO: Should we compare against already loaded assets here?
                    if (File.Exists(GetCachePath(asset.TargetHash)))
                    {
                        LiveLinkMsg.LogReceived($"AssetBundleTargetHash => {asset.GUID} | {asset.TargetHash}, File.Exists => 'True'");
                        _WaitingForAssets[asset.GUID] = asset.TargetHash;
                    }
                    else
                    {
                        LiveLinkMsg.LogReceived($"AssetBundleTargetHash => {asset.GUID} | {asset.TargetHash}, File.Exists => 'False'");
                        _WaitingForAssets[asset.GUID] = new Hash128();

                        LiveLinkMsg.LogSend($"AssetBundleBuild request '{asset.GUID}'");
                        PlayerConnection.instance.Send(LiveLinkMsg.RequestAssetBundleForGUID, asset.GUID);
                    }
                }
            }
        }

        protected override void OnUpdate()
        {
            // Request any new guids that we haven't seen yet from the editor
            using (var requestedGuids = _ResourceRequests.ToComponentDataArray<ResourceGUID>(Allocator.TempJob))
            {
                if (requestedGuids.Length > 0)
                {
                    EntityManager.AddComponent(_ResourceRequests, typeof(ResourceRequested));
                    LiveLinkMsg.LogSend($"AssetBundleTargetHash request {requestedGuids.Reinterpret<Hash128>().ToDebugString()}");
                    PlayerConnection.instance.SendArray(LiveLinkMsg.RequestAssetBundleTargetHash, requestedGuids);
                }
            }

            // * Ensure all assets we are waiting for have arrived.
            // * LoadAll asset bundles in one go when everything is ready
            if (_WaitingForAssets.Count != 0)
            {
                bool hasAllAssets = true;
                var assets = new NativeArray<ResolvedAssetID>(_WaitingForAssets.Count, Allocator.TempJob);
                int o = 0;
                foreach (var asset in _WaitingForAssets)
                {
                    if (asset.Value == new Hash128())
                        hasAllAssets = false;
                    assets[o++] = new ResolvedAssetID { GUID = asset.Key, TargetHash = asset.Value };
                }

                if (hasAllAssets)
                {
                    LoadAssetBundles(assets);
                    _WaitingForAssets.Clear();
                }

                assets.Dispose();
            }
        }

        public static void Reset()
        {
            _GlobalAssetObjectResolver.DisposeAssetBundles();
            _GlobalAssetObjectResolver = new GlobalAssetObjectResolver();

            foreach (var world in World.AllWorlds)
            {
                var system = world.GetExistingSystem<LiveLinkPlayerAssetRefreshSystem>();
                if (system != null)
                    system._WaitingForAssets.Clear();
            }
        }
    }
}