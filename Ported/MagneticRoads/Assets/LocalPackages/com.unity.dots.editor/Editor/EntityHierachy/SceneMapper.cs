using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Profiling;
using Unity.Scenes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.Entities.Editor
{
    interface ISceneMapper
    {
        Hash128 GetSubsceneHash(World world, Entity tagSceneEntity);

        bool TryGetSceneOrSubSceneInstanceId(Hash128 subSceneHash, out int instanceId);

        Hash128 GetParentSceneHash(Hash128 subSceneHash);
    }

    class SceneMapper : ISceneMapper, IDisposable
    {
        static readonly ProfilerMarker k_GetSceneSectionMappingProfilerMarker = new ProfilerMarker($"{nameof(SceneMapper)}.{nameof(GetSceneSectionMapping)}");
        static readonly ProfilerMarker k_RebuildSubsceneOwnershipMapProfilerMarker = new ProfilerMarker($"{nameof(SceneMapper)}.{nameof(RebuildSubsceneOwnershipMap)}");

        readonly Dictionary<Hash128, Hash128> m_SubsceneOwnershipMap = new Dictionary<Hash128, Hash128>(8);
        readonly Dictionary<Hash128, int> m_SceneAndSubSceneHashToGameObjectInstanceId = new Dictionary<Hash128, int>(8);

        Hash128 m_ScenesCountFingerprint;
        uint m_CachedSceneSectionMappingVersion;
        NativeHashMap<Entity, Entity> m_CachedSceneSectionMapping;

        public bool SceneManagerDirty { get; private set; }

        public Hash128 GetParentSceneHash(Hash128 subsceneGUID) => m_SubsceneOwnershipMap.TryGetValue(subsceneGUID, out var result) ? result : default;

        public Hash128 GetSubsceneHash(World world, Entity entity)
        {
            if (entity == Entity.Null)
                return default;

            var subsceneEntity = Entity.Null;
            var sceneSectionToSubSceneMap = GetSceneSectionMapping(world);
            if (sceneSectionToSubSceneMap.TryGetValue(entity, out var entityToSubscene))
            {
                // Entity is a scene section
                subsceneEntity = entityToSubscene;
            }
            else if (world.EntityManager.HasComponent<SceneTag>(entity))
            {
                // Entity is in a subscene
                var sceneEntity = world.EntityManager.GetSharedComponentData<SceneTag>(entity).SceneEntity;

                // Currently, it seems like SceneTag.SceneEntity does not point to an actual SceneEntity, but to a SceneSection. If so, use this reverse lookup.
                if (sceneSectionToSubSceneMap.TryGetValue(sceneEntity, out var sceneEntityToSubscene))
                    subsceneEntity = sceneEntityToSubscene;
                else
                    // Subscene may not be loaded
                    Debug.LogWarning($"Entity {world.EntityManager.GetName(entity)} has a {nameof(SceneTag)} component, but its subscene could not be found.");
            }

            if (subsceneEntity != Entity.Null)
            {
                if (world.EntityManager.HasComponent<SubScene>(subsceneEntity))
                    return world.EntityManager.GetComponentObject<SubScene>(subsceneEntity).SceneGUID;

                if (world.EntityManager.HasComponent<SceneReference>(subsceneEntity))
                    return world.EntityManager.GetComponentData<SceneReference>(subsceneEntity).SceneGUID;
            }

            return default;
        }

        public SceneMapper()
        {
            m_ScenesCountFingerprint = default;
            SceneManagerDirty = true; // Ensures cache rebuild on first tick
            LiveLinkConfigHelper.LiveLinkEnabledChanged += SetSceneManagerDirty;
            SceneManager.sceneLoaded += SetSceneManagerDirty;
            SceneManager.sceneUnloaded += SetSceneManagerDirty;
            EditorSceneManager.sceneOpened += SetSceneManagerDirty;
            EditorSceneManager.sceneClosed += SetSceneManagerDirty;
            EditorSceneManager.newSceneCreated += SetSceneManagerDirty;
            m_CachedSceneSectionMapping = new NativeHashMap<Entity, Entity>(10, Allocator.Persistent);
        }

        public void Dispose()
        {
            m_CachedSceneSectionMapping.Dispose();
            m_CachedSceneSectionMappingVersion = 0;
            LiveLinkConfigHelper.LiveLinkEnabledChanged -= SetSceneManagerDirty;
            SceneManager.sceneLoaded -= SetSceneManagerDirty;
            SceneManager.sceneUnloaded -= SetSceneManagerDirty;
            EditorSceneManager.sceneOpened -= SetSceneManagerDirty;
            EditorSceneManager.sceneClosed -= SetSceneManagerDirty;
            EditorSceneManager.newSceneCreated -= SetSceneManagerDirty;
        }

        void SetSceneManagerDirty(Scene scene) => SceneManagerDirty = true;
        void SetSceneManagerDirty(Scene scene, LoadSceneMode _) => SceneManagerDirty = true;
        void SetSceneManagerDirty(Scene scene, OpenSceneMode _) => SceneManagerDirty = true;
        void SetSceneManagerDirty(Scene scene, NewSceneSetup _, NewSceneMode __) => SceneManagerDirty = true;
        void SetSceneManagerDirty() => SceneManagerDirty = true;

        public void Update()
        {
            var newSceneCountFingerprint = new Hash128(
                (uint)SceneManager.sceneCount,
                (uint)EditorSceneManager.loadedSceneCount,
#if UNITY_2020_1_OR_NEWER
                (uint)EditorSceneManager.loadedRootSceneCount,
#else
                0,
#endif
                (uint)EditorSceneManager.previewSceneCount);

            if (SceneManagerDirty || m_ScenesCountFingerprint != newSceneCountFingerprint)
            {
                RebuildSubsceneOwnershipMap();
                m_ScenesCountFingerprint = newSceneCountFingerprint;
                SceneManagerDirty = false;
            }
        }

        public bool TryGetSceneOrSubSceneInstanceId(Hash128 sceneHash, out int instanceId)
            => m_SceneAndSubSceneHashToGameObjectInstanceId.TryGetValue(sceneHash, out instanceId);

        void RebuildSubsceneOwnershipMap()
        {
            using (k_RebuildSubsceneOwnershipMapProfilerMarker.Auto())
            using (var sceneToHandleMap = PooledDictionary<Hash128, int>.Make())
            using (var subSceneToInstanceIdMap = PooledDictionary<Hash128, int>.Make())
            {
                m_SubsceneOwnershipMap.Clear();
                m_SceneAndSubSceneHashToGameObjectInstanceId.Clear();
                for (var i = 0; i < SceneManager.sceneCount; ++i)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if (!scene.isLoaded)
                        continue;

                    var sceneHash = new Hash128(AssetDatabase.AssetPathToGUID(scene.path));

                    // A scene handle is equivalent to an instance id for a scene.
                    sceneToHandleMap.Dictionary.Add(sceneHash, scene.handle);

                    using (var rootGameObjects = PooledList<GameObject>.Make())
                    using (var subSceneComponents = PooledList<SubScene>.Make())
                    {
                        scene.GetRootGameObjects(rootGameObjects);
                        foreach (var go in rootGameObjects.List)
                        {
                            go.GetComponentsInChildren(subSceneComponents.List);
                            foreach (var subSceneComponent in subSceneComponents.List)
                            {
                                if (subSceneComponent.SceneAsset != null)
                                {
                                    // There could be more than one scene referencing the same subscene, but it is not legal and already throws. Just ignore it here.
                                    if (!m_SubsceneOwnershipMap.ContainsKey(subSceneComponent.SceneGUID))
                                    {
                                        m_SubsceneOwnershipMap.Add(subSceneComponent.SceneGUID, sceneHash);
                                        subSceneToInstanceIdMap.Dictionary.Add(subSceneComponent.SceneGUID, go.GetInstanceID());
                                    }
                                }
                            }
                            subSceneComponents.List.Clear();
                        }
                    }
                }

                foreach (var kvp in subSceneToInstanceIdMap.Dictionary)
                {
                    m_SceneAndSubSceneHashToGameObjectInstanceId[kvp.Key] = kvp.Value;
                }

                foreach (var kvp in sceneToHandleMap.Dictionary)
                {
                    if (!m_SceneAndSubSceneHashToGameObjectInstanceId.ContainsKey(kvp.Key))
                        m_SceneAndSubSceneHashToGameObjectInstanceId[kvp.Key] = kvp.Value;
                }
            }
        }

        NativeHashMap<Entity, Entity> GetSceneSectionMapping(World world)
        {
            if (world.EntityManager.GlobalSystemVersion == m_CachedSceneSectionMappingVersion)
                return m_CachedSceneSectionMapping;

            m_CachedSceneSectionMappingVersion = world.EntityManager.GlobalSystemVersion;
            m_CachedSceneSectionMapping.Clear();

            using (k_GetSceneSectionMappingProfilerMarker.Auto())
            {
                var query = world.EntityManager.CreateEntityQuery(typeof(ResolvedSectionEntity));
                var entities = query.ToEntityArrayAsync(Allocator.TempJob, out var handle);
                var job = new GetSceneSectionMappingJob
                {
                    Entities = entities,
                    BufferAccessor = world.EntityManager.GetBufferFromEntity<ResolvedSectionEntity>(isReadOnly: true),
                    SceneSectionToSubsceneMap = m_CachedSceneSectionMapping
                }.Schedule(handle);
                job.Complete();
                query.Dispose();

                return m_CachedSceneSectionMapping;
            }
        }

        [BurstCompile]
        struct GetSceneSectionMappingJob : IJob
        {
            [ReadOnly] public BufferFromEntity<ResolvedSectionEntity> BufferAccessor;
            [ReadOnly, DeallocateOnJobCompletion] public NativeArray<Entity> Entities;
            [WriteOnly] public NativeHashMap<Entity, Entity> SceneSectionToSubsceneMap;

            public void Execute()
            {
                for (var i = 0; i < Entities.Length; i++)
                {
                    var entity = Entities[i];
                    var buffer = BufferAccessor[entity];
                    for (var j = 0; j < buffer.Length; j++)
                    {
                        SceneSectionToSubsceneMap.TryAdd(buffer[j].SectionEntity, entity);
                    }
                }
            }
        }
    }
}
