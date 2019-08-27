#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hash128 = Unity.Entities.Hash128;

namespace Unity.Scenes
{
    public enum LiveLinkMode
    {
        Disabled = 0,
        ConvertWithoutDiff,
        LiveConvertGameView,
        LiveConvertSceneView,        
    }
    
    public class LiveLinkScene
    {
        public int LiveLinkDirtyID = -1;
        public World ConvertedWorld;
        public World GameObjectWorld;
        public World DestinationWorld;

        EntityManagerPatcher LiveLinkPatcher;
        private EntityManagerDiffer LiveLinkDiffer;

        HashSet<GameObject> _ChangedGameObjects = new HashSet<GameObject>();
        bool                _RequestCleanConversion;
        LiveLinkMode        _LiveLinkMode;
        
        public void AddChanged(GameObject gameObject)
        {
            // Debug.Log("AddChanged");
            _ChangedGameObjects.Add(gameObject);
        }

        public void RequestCleanConversion()
        {
            // Debug.Log("RequestCleanConversion");
            _RequestCleanConversion = true;
        }

        
        public bool DidRequestUpdate()
        {
            return _RequestCleanConversion || _ChangedGameObjects.Count != 0;
        }

        public LiveLinkScene(World destinationWorld, LiveLinkMode mode)
        {
            DestinationWorld = destinationWorld;
            ConvertedWorld = new World("Clean Entity Conversion World");
            LiveLinkDiffer = new EntityManagerDiffer(ConvertedWorld, Allocator.Persistent);
            LiveLinkPatcher = new EntityManagerPatcher(destinationWorld, Allocator.Persistent);
            _RequestCleanConversion = true;
            _LiveLinkMode = mode;
        }

        public void Dispose()
        {
            LiveLinkPatcher.Dispose();
            LiveLinkDiffer.Dispose();
            
            if (GameObjectWorld != null&& GameObjectWorld.IsCreated)
                GameObjectWorld.Dispose();
            GameObjectWorld = null;
            
            if (ConvertedWorld != null&& ConvertedWorld.IsCreated)
                ConvertedWorld.Dispose();
            ConvertedWorld = null;
        }

        static ProfilerMarker m_Convert = new ProfilerMarker("LiveLink.Convert");

        public void Convert(Scene scene, Hash128 sceneHash, GameObjectConversionUtility.ConversionFlags flags)
        {
            using (m_Convert.Auto())
            {
                // Try incremental conversion
                if (!_RequestCleanConversion)
                {
                    // Debug.Log("Incremental convert");
                    try
                    {
                        GameObjectConversionUtility.ConvertIncremental(GameObjectWorld, _ChangedGameObjects, flags);
                        _ChangedGameObjects.Clear();
                    }
                    catch (Exception e)
                    {
                        _RequestCleanConversion = true;
                        Debug.LogWarning("Incremental conversion failed. Performing full conversion instead\n" + e.ToString());
                    }
                }

                // If anything failed, fall back to clean conversion
                if (_RequestCleanConversion)
                {
                    // Debug.Log("Clean convert");
                    ConvertedWorld.EntityManager.DestroyEntity(ConvertedWorld.EntityManager.UniversalQuery);
                    GameObjectWorld = GameObjectConversionUtility.ConvertIncrementalInitialize(scene, new GameObjectConversionSettings(ConvertedWorld, sceneHash, flags));
                }
                
                _ChangedGameObjects.Clear();
                _RequestCleanConversion = false;
            }
        }
        
        public static void ApplyLiveLink(SubScene scene, World dstWorld, int sceneDirtyID, LiveLinkMode mode)
        {
            //Debug.Log("ApplyLiveLink: " + scene.SceneName);

            #if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (mode == LiveLinkMode.Disabled)
                throw new System.ArgumentException("LiveLinkMode must not be disabled");
            #endif

            // When switching mode, completely unload the previous scene
            if (scene.LiveLinkData != null && (scene.LiveLinkData._LiveLinkMode == LiveLinkMode.ConvertWithoutDiff || mode == LiveLinkMode.ConvertWithoutDiff || scene.LiveLinkData.DestinationWorld != dstWorld))
            {
                scene.LiveLinkData.Dispose();
                scene.LiveLinkData = null;
            }

            var unloadAllPreviousEntities = scene.LiveLinkData == null;
            if (scene.LiveLinkData == null)
                scene.LiveLinkData = new LiveLinkScene(dstWorld, mode);
            var liveLink = scene.LiveLinkData; 


            var flags = GameObjectConversionUtility.ConversionFlags.AddEntityGUID | GameObjectConversionUtility.ConversionFlags.AssignName | GameObjectConversionUtility.ConversionFlags.GameViewLiveLink;
            if (mode == LiveLinkMode.LiveConvertSceneView)
                flags |= GameObjectConversionUtility.ConversionFlags.SceneViewLiveLink;
            
            liveLink.Convert(scene.LoadedScene, scene.SceneGUID, flags);

            
            var streamingSystem = dstWorld.GetExistingSystem<SubSceneStreamingSystem>();
            var dstEntities = dstWorld.EntityManager;

            // Unload scene
            if (unloadAllPreviousEntities)
            {
                foreach (var s in scene._SceneEntities)
                {
                    streamingSystem.UnloadSceneImmediate(s);
                    dstEntities.DestroyEntity(s);
                }

                var sceneEntity = dstEntities.CreateEntity();
                dstEntities.SetName(sceneEntity, "Scene (LiveLink): " + scene.SceneName);
                dstEntities.AddComponentObject(sceneEntity, scene);
                dstEntities.AddComponentData(sceneEntity, new SubSceneStreamingSystem.StreamingState { Status = SubSceneStreamingSystem.StreamingStatus.Loaded});
                dstEntities.AddComponentData(sceneEntity, new SubSceneStreamingSystem.IgnoreTag( ));

                scene._SceneEntities = new List<Entity>();
                scene._SceneEntities.Add(sceneEntity);
            }


            var convertedEntityManager = liveLink.ConvertedWorld.EntityManager;

            var liveLinkSceneEntity = scene._SceneEntities[0];


            /// We want to let the live linked scene be able to reference the already existing Scene Entity (Specifically SceneTag should point to the scene Entity after live link completes)
            // Add Scene tag to all entities using the convertedSceneEntity that will map to the already existing scene entity.
            using (var missingSceneQuery = convertedEntityManager.CreateEntityQuery(new EntityQueryDesc
            {
                None = new ComponentType[] {typeof(SceneTag)},
                Options = EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabled
            }))
            {
                convertedEntityManager.AddSharedComponentData(missingSceneQuery, new SceneTag { SceneEntity = liveLinkSceneEntity });
            }

            if (mode != LiveLinkMode.ConvertWithoutDiff)
            {
                var options = EntityManagerDifferOptions.IncludeForwardChangeSet | 
                              EntityManagerDifferOptions.FastForwardShadowWorld | 
                              EntityManagerDifferOptions.ValidateUniqueEntityGuid | 
                              EntityManagerDifferOptions.ClearMissingReferences;

                using (var changes = liveLink.LiveLinkDiffer.GetChanges(options, Allocator.TempJob))
                {
                    liveLink.LiveLinkPatcher.ApplyChangeSet(changes.ForwardChangeSet);
                }
            }
            else
            {
                dstWorld.EntityManager.MoveEntitiesFrom(liveLink.ConvertedWorld.EntityManager);
                
            }
            
            // convertedEntityManager.Debug.CheckInternalConsistency();
            //liveLink.ConvertedShadowWorld.EntityManager.Debug.CheckInternalConsistency();

            using (var missingRenderData = dstEntities.CreateEntityQuery(new EntityQueryDesc
                {
                    All = new ComponentType[] {typeof(SceneTag)},
                    None = new ComponentType[] {typeof(EditorRenderData)},
                    Options = EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabled
                }))
            {
                missingRenderData.SetFilter(new SceneTag {SceneEntity = liveLinkSceneEntity});
                dstEntities.AddSharedComponentData(missingRenderData, new EditorRenderData() { SceneCullingMask = EditorRenderData.LiveLinkEditGameViewMask, PickableObject = scene.gameObject });
            }

            liveLink.LiveLinkDirtyID = sceneDirtyID;
            EditorUpdateUtility.EditModeQueuePlayerLoopUpdate();
        }
    }
}
#endif