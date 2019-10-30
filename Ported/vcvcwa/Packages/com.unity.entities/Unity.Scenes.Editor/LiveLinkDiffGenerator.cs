using System;
using System.Collections.Generic;
using System.IO;
using Unity.Build;
using Unity.Collections;
using Unity.Entities;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hash128 = Unity.Entities.Hash128;

namespace Unity.Scenes.Editor
{
    //@TODO: Live link currently force loads all sections. It should only live link sections that are marked for loading.
    class LiveLinkDiffGenerator : IDisposable
    {
        public int                  LiveLinkDirtyID = -1;

        World                       _GameObjectWorld;

        World                       _ConvertedWorld;
        EntityQuery                 _MissingRenderDataQuery;
        EntityQuery                 _MissingSceneQuery;
        EntityManagerDiffer         _LiveLinkDiffer;

        HashSet<GameObject>         _ChangedGameObjects = new HashSet<GameObject>();
        bool                        _RequestCleanConversion;
        bool                        _LiveLinkEnabled;

        string                      _SceneName;

        BlobAssetStore     m_BlobAssetStore = new BlobAssetStore();
        
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

        public LiveLinkDiffGenerator(Hash128 sceneGUID, bool liveLinkEnabled)
        {
            _SceneName = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(sceneGUID.ToString()));

            _LiveLinkEnabled = liveLinkEnabled;
            _ConvertedWorld = new World($"Converted Scene: '{_SceneName}");
            _LiveLinkDiffer = new EntityManagerDiffer(_ConvertedWorld.EntityManager, Allocator.Persistent);
            _RequestCleanConversion = true;

            _MissingRenderDataQuery = _ConvertedWorld.EntityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] {typeof(SceneTag)},
                None = new ComponentType[] {typeof(EditorRenderData)},
                Options = EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabled
            });
            
            _MissingSceneQuery = _ConvertedWorld.EntityManager.CreateEntityQuery(new EntityQueryDesc
            {
                None = new ComponentType[] {typeof(SceneTag)},
                Options = EntityQueryOptions.IncludePrefab | EntityQueryOptions.IncludeDisabled
            });
        }
        
        public void Dispose()
        {
            m_BlobAssetStore.Dispose();
            
            try
            {
                _LiveLinkDiffer.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            try
            {
                if (_GameObjectWorld != null && _GameObjectWorld.IsCreated)
                    _GameObjectWorld.Dispose();
                _GameObjectWorld = null;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        
            try
            {
                if (_ConvertedWorld != null && _ConvertedWorld.IsCreated)
                    _ConvertedWorld.Dispose();
                _ConvertedWorld = null;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        static ProfilerMarker m_ConvertMarker = new ProfilerMarker("LiveLink.Convert");

        public void Convert(Scene scene, Hash128 sceneGUID, GameObjectConversionUtility.ConversionFlags flags, BuildSettings buildSettings)
        {
            using (m_ConvertMarker.Auto())
            {
                // Try incremental conversion
                if (!_RequestCleanConversion)
                {
                    // Debug.Log("Incremental convert");
                    try
                    {
                        GameObjectConversionUtility.ConvertIncremental(_GameObjectWorld, _ChangedGameObjects, flags);
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
                    _ConvertedWorld.EntityManager.DestroyEntity(_ConvertedWorld.EntityManager.UniversalQuery);
                    var conversionSettings = new GameObjectConversionSettings(_ConvertedWorld, flags);
                    conversionSettings.BuildSettings = buildSettings;
                    conversionSettings.SceneGUID = sceneGUID;
                    conversionSettings.DebugConversionName = _SceneName;
                    conversionSettings.blobAssetStore = m_BlobAssetStore;
                    
                    if (_GameObjectWorld != null && _GameObjectWorld.IsCreated)
                    {
                        _GameObjectWorld.Dispose();
                        _GameObjectWorld = null;
                    }
                    _GameObjectWorld = GameObjectConversionUtility.ConvertIncrementalInitialize(scene, conversionSettings);
                }

                _ChangedGameObjects.Clear();
                _RequestCleanConversion = false;
            }
        }

        public static LiveLinkChangeSet UpdateLiveLink(Scene scene, Hash128 sceneGUID, ref LiveLinkDiffGenerator liveLinkData, int sceneDirtyID, LiveLinkMode mode, BuildSettings buildSettings)
        {
            //Debug.Log("ApplyLiveLink: " + scene.SceneName);

            var liveLinkEnabled = mode != LiveLinkMode.Disabled;
            if (liveLinkData != null && liveLinkData._LiveLinkEnabled != liveLinkEnabled)
            {
                liveLinkData.Dispose();
                liveLinkData = null;
            }
            
            var unloadAllPreviousEntities = liveLinkData == null;
            if (liveLinkData == null)
                liveLinkData = new LiveLinkDiffGenerator(sceneGUID, liveLinkEnabled);
            
            if (!liveLinkEnabled)
            {
                return new LiveLinkChangeSet
                {
                    UnloadAllPreviousEntities = unloadAllPreviousEntities,
                    SceneName = scene.name,
                    SceneGUID = sceneGUID
                };
            }

            var flags = GameObjectConversionUtility.ConversionFlags.AddEntityGUID | GameObjectConversionUtility.ConversionFlags.AssignName | GameObjectConversionUtility.ConversionFlags.GameViewLiveLink;
            if (mode == LiveLinkMode.LiveConvertSceneView)
                flags |= GameObjectConversionUtility.ConversionFlags.SceneViewLiveLink;

            liveLinkData.Convert(scene, sceneGUID, flags, buildSettings);

            var convertedEntityManager = liveLinkData._ConvertedWorld.EntityManager;

            // We don't know the scene tag of the destination world, so we create a null Scene Tag.
            // In the patching code this will be translated into the final scene entity.
            convertedEntityManager.AddSharedComponentData(liveLinkData._MissingSceneQuery, new SceneTag { SceneEntity = Entity.Null });

#if UNITY_2020_1_OR_NEWER
            convertedEntityManager.AddSharedComponentData(liveLinkData._MissingRenderDataQuery, new EditorRenderData { SceneCullingMask = UnityEditor.SceneManagement.SceneCullingMasks.GameViewObjects, PickableObject = null });
#else
            convertedEntityManager.AddSharedComponentData(liveLinkData._MissingRenderDataQuery, new EditorRenderData { SceneCullingMask = EditorRenderData.LiveLinkEditGameViewMask, PickableObject = null });
#endif

            var options = EntityManagerDifferOptions.IncludeForwardChangeSet | 
                          EntityManagerDifferOptions.FastForwardShadowWorld | 
                          EntityManagerDifferOptions.ValidateUniqueEntityGuid | 
                          EntityManagerDifferOptions.ClearMissingReferences;

            var changes = new LiveLinkChangeSet
            {
                Changes = liveLinkData._LiveLinkDiffer.GetChanges(options, Allocator.TempJob).ForwardChangeSet,
                UnloadAllPreviousEntities = unloadAllPreviousEntities,
                SceneName = scene.name,
                SceneGUID = sceneGUID
            };
                
                
            liveLinkData.LiveLinkDirtyID = sceneDirtyID;
            // convertedEntityManager.Debug.CheckInternalConsistency();

            return changes;
        }
    }
}