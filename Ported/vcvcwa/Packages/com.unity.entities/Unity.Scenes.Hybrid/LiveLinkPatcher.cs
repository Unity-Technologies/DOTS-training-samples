using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Serialization;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;

namespace Unity.Scenes
{
    public enum LiveLinkMode
    {
        Disabled = 0,
        LiveConvertGameView,
        LiveConvertSceneView,        
    }

    struct LiveLinkChangeSet : IDisposable
    {
        public Hash128         SceneGUID;
        public EntityChangeSet Changes;
        public string          SceneName;
        public bool            UnloadAllPreviousEntities;

        public void Dispose()
        {
            Changes.Dispose();
        }

        #if UNITY_EDITOR
        public byte[] Serialize()
        {
            var buffer = new UnsafeAppendBuffer(1024, 16, Allocator.Persistent);

            EntityChangeSetSerialization.ResourcePacket.SerializeResourcePacket(Changes, ref buffer);
            
            buffer.Add(SceneGUID);
            buffer.Add(SceneName);
            buffer.Add(UnloadAllPreviousEntities);

            return buffer.ToBytes();
        }
        #endif

        unsafe public static LiveLinkChangeSet Deserialize(EntityChangeSetSerialization.ResourcePacket resource, GlobalAssetObjectResolver resolver)
        {
            var reader = resource.ChangeSet.AsReader();

            LiveLinkChangeSet changeSet;
            changeSet.Changes = EntityChangeSetSerialization.Deserialize(&reader, resource.GlobalObjectIds, resolver);
            reader.ReadNext(out changeSet.SceneGUID);
            reader.ReadNext(out changeSet.SceneName);
            reader.ReadNext(out changeSet.UnloadAllPreviousEntities);

            return changeSet;
        }
    }


    class LiveLinkPatcher
    {
        public struct LiveLinkedSceneState : ISystemStateComponentData, IEquatable<LiveLinkedSceneState>
        {
            public Hash128 Scene;

            public bool Equals(LiveLinkedSceneState other)
            {
                return Scene.Equals(other.Scene);
            }
            public override int GetHashCode()
            {
                return Scene.GetHashCode();
            }
        }

        
        private World _DstWorld;
        EntityQuery _AddedScenesQuery;
        private EntityQuery _RemovedScenesQuery;
        public LiveLinkPatcher(World destinationWorld)
        {
            _DstWorld = destinationWorld;
            
            _AddedScenesQuery = _DstWorld.EntityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] {typeof(SceneTag)},
                Options = EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabled
            });
            _AddedScenesQuery.SetSharedComponentFilter(new SceneTag { SceneEntity = Entity.Null});

            _RemovedScenesQuery = _DstWorld.EntityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] {typeof(LiveLinkedSceneState)},
                None = new ComponentType[] {typeof(SceneReference)},
            });
        }

        public void Dispose()
        {
            _AddedScenesQuery.Dispose();
            _RemovedScenesQuery.Dispose();
        }

        struct RemoveLiveLinkSceneState : IJobForEachWithEntity<LiveLinkedSceneState>
        {
            public Hash128 DeleteGuid;
            public  EntityCommandBuffer Commands;
            public void Execute(Entity entity, int offset, ref LiveLinkedSceneState scene)
            {
                if (scene.Scene == DeleteGuid)
                    Commands.RemoveComponent<LiveLinkedSceneState>(entity);
            }
        }

        public void UnloadScene(Hash128 sceneGUID)
        {
            var dstEntities = _DstWorld.EntityManager;
            var sceneSystem = _DstWorld.GetExistingSystem<SceneSystem>();

            var sceneEntity = sceneSystem.GetSceneEntity(sceneGUID);
            
            dstEntities.RemoveComponent<DisableSceneResolveAndLoad>(sceneEntity);
            dstEntities.RemoveComponent<LiveLinkedSceneState>(sceneEntity);
            sceneSystem.UnloadScene(sceneEntity, SceneSystem.UnloadParameters.DestroySectionProxyEntities | SceneSystem.UnloadParameters.DontRemoveRequestSceneLoaded);


            // Cleanup leftover LiveLinkedScene system state
            // (This happens if the scene entity got destroyed)
            var job = new RemoveLiveLinkSceneState
            {
                DeleteGuid = sceneGUID,
                Commands = new EntityCommandBuffer(Allocator.TempJob)
            };
            job.Run(_RemovedScenesQuery);
            job.Commands.Playback(dstEntities);
            job.Commands.Dispose();
        }

        public void ApplyPatch(LiveLinkChangeSet changeSet)
        {
            var dstEntities = _DstWorld.EntityManager;
            var sceneSystem = _DstWorld.GetExistingSystem<SceneSystem>();
            Entity sectionEntity;
            var sceneEntity = sceneSystem.GetSceneEntity(changeSet.SceneGUID);

            //@TODO: Check if the scene or section is requested to be loaded
            if (sceneEntity == Entity.Null)
            {
                Debug.LogWarning($"'{changeSet.SceneName}' ({{changeSet.sceneGUID}}) was ignored in live link since it is not loaded.");
                return;
            }

            // Unload scene
            if (changeSet.UnloadAllPreviousEntities)
            {
                //@Todo: Can we try to keep scene & section entities alive? (In case user put custom data on it)
                sceneSystem.UnloadScene(sceneEntity, SceneSystem.UnloadParameters.DestroySectionProxyEntities | SceneSystem.UnloadParameters.DontRemoveRequestSceneLoaded);

                // Create section
                sectionEntity = dstEntities.CreateEntity();
                dstEntities.AddComponentData(sectionEntity, new SceneSectionStreamingSystem.StreamingState { Status = SceneSectionStreamingSystem.StreamingStatus.Loaded});
                dstEntities.AddComponentData(sectionEntity, new DisableSceneResolveAndLoad( ));

                // Configure scene
                dstEntities.AddComponentData(sceneEntity, new DisableSceneResolveAndLoad( ));
                dstEntities.AddComponentData(sceneEntity, new LiveLinkedSceneState { Scene = changeSet.SceneGUID });

                dstEntities.AddBuffer<ResolvedSectionEntity>(sceneEntity).Add(new ResolvedSectionEntity { SectionEntity = sectionEntity} );
                
#if UNITY_EDITOR
                dstEntities.SetName(sectionEntity, "SceneSection (LiveLink): " + changeSet.SceneName);
                dstEntities.SetName(sceneEntity, "Scene (LiveLink): " + changeSet.SceneName);
#endif
            }
            else
            {
                sectionEntity = dstEntities.GetBuffer<ResolvedSectionEntity>(sceneEntity)[0].SectionEntity;
            }
            
            // SceneTag.SceneEntity == Entity.Null is reserved for new entities added via live link.
            if (_AddedScenesQuery.CalculateChunkCount() != 0)
            {
                Debug.LogWarning("SceneTag.SceneEntity must not reference Entity.Null. Destroying Entities.");
                dstEntities.DestroyEntity(_AddedScenesQuery);
            }
            
            EntityPatcher.ApplyChangeSet(_DstWorld.EntityManager, changeSet.Changes);
            
            //liveLink.ConvertedShadowWorld.EntityManager.Debug.CheckInternalConsistency();

            dstEntities.SetSharedComponentData(_AddedScenesQuery, new SceneTag { SceneEntity = sectionEntity });

            EditorUpdateUtility.EditModeQueuePlayerLoopUpdate();
        }
    }
}