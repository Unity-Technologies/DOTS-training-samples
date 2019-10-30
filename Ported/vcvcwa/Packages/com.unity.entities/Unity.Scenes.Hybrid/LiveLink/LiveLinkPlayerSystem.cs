using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Networking.PlayerConnection;
using Object = UnityEngine.Object;

namespace Unity.Scenes
{
    struct ResourceGUID : IComponentData
    {
        public Hash128 Guid;
    }

    struct ResourceLoaded : IComponentData
    {
    }

#if UNITY_EDITOR
    [DisableAutoCreation]
#endif
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(LiveLinkRuntimeSystemGroup))]
    class LiveLinkPlayerSystem : ComponentSystem
    {
        Queue<EntityChangeSetSerialization.ResourcePacket> m_ResourcePacketQueue;
        LiveLinkPatcher                                    m_Patcher;
        EntityQuery                                        m_Resources;
        EntityQuery                                        m_AllScenes;
        LiveLinkSceneChangeTracker                         m_LiveLinkSceneChange;
        bool                                               m_DidRequestConnection;

        HashSet<Hash128>                                   m_LiveLinkedScenes = new HashSet<Hash128>();
        
        protected override void OnStartRunning()
        {
            Debug.Log("Initializing live link");
            
            PlayerConnection.instance.Register(LiveLinkMsg.ReceiveEntityChangeSet, ReceiveEntityChangeSet);
            PlayerConnection.instance.Register(LiveLinkMsg.UnloadScenes, ReceiveUnloadScenes);
            PlayerConnection.instance.Register(LiveLinkMsg.LoadScenes, ReceiveLoadScenes);
            PlayerConnection.instance.Register(LiveLinkMsg.ResetGame, ReceiveResetGame);

            m_ResourcePacketQueue = new Queue<EntityChangeSetSerialization.ResourcePacket>();
            m_Resources = GetEntityQuery( new EntityQueryDesc
            {
                All = new [] { ComponentType.ReadOnly<ResourceGUID>() }
            });

            m_AllScenes = GetEntityQuery( new EntityQueryDesc
            {
                All = new [] { ComponentType.ReadOnly<SceneReference>() }
            });

            m_Patcher = new LiveLinkPatcher(World);
            
            m_LiveLinkSceneChange = new LiveLinkSceneChangeTracker(EntityManager);
        }

        protected override void OnStopRunning()
        {
            m_LiveLinkSceneChange.Dispose();
            PlayerConnection.instance.Unregister(LiveLinkMsg.ReceiveEntityChangeSet, ReceiveEntityChangeSet);
            PlayerConnection.instance.Unregister(LiveLinkMsg.UnloadScenes, ReceiveUnloadScenes);
            PlayerConnection.instance.Unregister(LiveLinkMsg.LoadScenes, ReceiveLoadScenes);
            PlayerConnection.instance.Unregister(LiveLinkMsg.ResetGame, ReceiveResetGame);
        }

        void SendSetLoadedScenes()
        {
            if (m_LiveLinkSceneChange.GetSceneMessage(out var msg))
            {
                LiveLinkMsg.LogSend($"SetLoadedScenes: Loaded {msg.LoadedScenes.ToDebugString()}, Removed {msg.RemovedScenes.ToDebugString()}");
                PlayerConnection.instance.Send(LiveLinkMsg.SetLoadedScenes, msg.ToMsg());
                msg.Dispose();
            }
        }

        unsafe void ReceiveEntityChangeSet(MessageEventArgs args)
        {
            var resourcePacket = new EntityChangeSetSerialization.ResourcePacket(args.data);

            LiveLinkMsg.LogReceived($"EntityChangeSet patch: '{args.data.Length}' bytes, " +
                                    $"object GUIDs: {resourcePacket.GlobalObjectIds.ToDebugString(id => id.AssetGUID.ToString())}");

            m_ResourcePacketQueue.Enqueue(resourcePacket);
        }

        unsafe void ReceiveUnloadScenes(MessageEventArgs args)
        {
            using (var scenes = args.ReceiveArray<Hash128>())
            {
                LiveLinkMsg.LogReceived($"UnloadScenes {scenes.ToDebugString()}");
                foreach (var scene in scenes)
                {
                    m_Patcher.UnloadScene(scene);
                    m_LiveLinkedScenes.Remove(scene);
                }
            }
        }
        
        unsafe void ReceiveLoadScenes(MessageEventArgs args)
        {
            using (var scenes = args.ReceiveArray<Hash128>())
            {
                LiveLinkMsg.LogReceived($"LoadScenes {scenes.ToDebugString()}");
                foreach (var scene in scenes)
                {
                    m_LiveLinkedScenes.Add(scene);
                }
            }
        }
        
        void ReceiveResetGame(MessageEventArgs args)
        {
            LiveLinkMsg.LogReceived("ResetGame");
            ResetGame();
        }

        void ResetGame()
        {
            while (m_ResourcePacketQueue.Count != 0)
                m_ResourcePacketQueue.Dequeue().Dispose();

            EntityManager.DestroyEntity(EntityManager.UniversalQuery);
            
            //@TODO: Once we have build settings & loading of game object scenes we can remove this hack.
            var scenes = Object.FindObjectsOfType<SubScene>();
            foreach (var scene in scenes)
            {
                scene.enabled = false;
                scene.enabled = true;
            }
            
            LiveLinkPlayerAssetRefreshSystem.Reset();
            
            LiveLinkMsg.LogSend("ConnectLiveLink");
            PlayerConnection.instance.Send(LiveLinkMsg.ConnectLiveLink, World.GetExistingSystem<SceneSystem>().BuildSettingsGUID);
            
            m_LiveLinkSceneChange.Reset();
            SendSetLoadedScenes();
        }

        [BurstCompile]
        struct BuildResourceMapJob : IJobForEachWithEntity<ResourceGUID>
        {
            [ReadOnly]
            public ComponentDataFromEntity<ResourceLoaded> ResourceLoaded;

            public NativeHashMap<Entities.Hash128, byte> GuidResourceReady;

            public void Execute(Entity entity, int index, ref ResourceGUID resourceGuid)
            {
                var guid = resourceGuid.Guid;
                GuidResourceReady[guid] = (byte)(ResourceLoaded.HasComponent(entity) ? 1 : 0);
            }
        }
        
        bool IsResourceReady(EntityChangeSetSerialization.ResourcePacket packet)
        {
            var entityChangeSetWithResources = packet;
            var resourceGuids = entityChangeSetWithResources.GlobalObjectIds;

            var guidResourceReady = new NativeHashMap<Entities.Hash128,byte>(m_Resources.CalculateEntityCount(), Allocator.Persistent);

            new BuildResourceMapJob
            {
                ResourceLoaded = GetComponentDataFromEntity<ResourceLoaded>(),
                GuidResourceReady = guidResourceReady
            }.Run(m_Resources);
            
            var isResourceReady = true;
            var archetype = EntityManager.CreateArchetype(typeof(ResourceGUID));

            for (int i = 0; i < resourceGuids.Length; i++)
            {
                var guid = resourceGuids[i].AssetGUID;
                var found = guidResourceReady.ContainsKey(guid);
                if (!found)
                {
                    guidResourceReady.TryAdd(guid, 0);

                    var entity = EntityManager.CreateEntity(archetype);
                    EntityManager.SetComponentData(entity, new ResourceGUID
                    {
                        Guid = guid
                    });
                }

                var ready = guidResourceReady[guid] == 1;
                if (!ready)
                {
                    isResourceReady = false;
                }
            }
            
            guidResourceReady.Dispose();
            return isResourceReady;
        }

        protected override void OnUpdate()
        {
            var sceneSystem = World.GetExistingSystem<SceneSystem>();

            Entities.With(m_AllScenes).ForEach((Entity e, ref SceneReference sceneRef) =>
                {
                    if(m_LiveLinkedScenes.Contains(sceneRef.SceneGUID))
                        EntityManager.AddComponent<DisableSceneResolveAndLoad>(e);
                    else
                        EntityManager.RemoveComponent<DisableSceneResolveAndLoad>(e);
                }
            );
            
            
            // BuildSettingsGUID isn't known in OnCreate since it could be configured from OnCreate of other systems,
            // So we delay connecting live link until first OnUpdate
            if (!m_DidRequestConnection)
            {
                m_DidRequestConnection = true;
                LiveLinkMsg.LogSend("ConnectLiveLink");
                PlayerConnection.instance.Send(LiveLinkMsg.ConnectLiveLink, World.GetExistingSystem<SceneSystem>().BuildSettingsGUID);
            }

            SendSetLoadedScenes();

            while (m_ResourcePacketQueue.Count != 0 && IsResourceReady(m_ResourcePacketQueue.Peek()))
            {
                LiveLinkMsg.LogInfo($"Applying changeset ({m_ResourcePacketQueue.Count-1} left in queue)");
                using (var resourcePacket = m_ResourcePacketQueue.Dequeue())
                {
                    ApplyChangeSet(resourcePacket);
                }
            }
        }

        unsafe void ApplyChangeSet(EntityChangeSetSerialization.ResourcePacket resourcePacket)
        {
            var changeSet = LiveLinkChangeSet.Deserialize(resourcePacket, LiveLinkPlayerAssetRefreshSystem.GlobalAssetObjectResolver);
            m_Patcher.ApplyPatch(changeSet);
            changeSet.Dispose();
        }
    }
}