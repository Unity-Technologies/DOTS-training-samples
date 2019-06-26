using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Profiling;

namespace Unity.Rendering
{
    [BurstCompile]
    struct GatherChunkRenderers : IJobParallelFor
    {
        [ReadOnly] public NativeArray<ArchetypeChunk> Chunks;
        [ReadOnly] public ArchetypeChunkSharedComponentType<RenderMesh> RenderMeshType;
        public NativeArray<int> ChunkRenderer;

        public void Execute(int chunkIndex)
        {
            var chunk = Chunks[chunkIndex];
            var sharedIndex = chunk.GetSharedComponentIndex(RenderMeshType);
            ChunkRenderer[chunkIndex] = sharedIndex;
        }
    }


    struct SubSceneTagOrderVersion
    {
        public FrozenRenderSceneTag Scene;
        public int Version;
    }

    public unsafe struct CullingStats
    {
        public const int kChunkTotal = 0;
        public const int kChunkCountAnyLod = 1;
        public const int kChunkCountInstancesProcessed = 2;
        public const int kChunkCountFullyIn = 3;
        public const int kInstanceTests = 4;
        public const int kLodTotal = 5;
        public const int kLodNoRequirements = 6;
        public const int kLodChanged = 7;
        public const int kLodChunksTested = 8;
        public const int kCountRootLodsSelected = 9;
        public const int kCountRootLodsFailed = 10;
        public const int kCount = 11;
        public fixed int Stats[kCount];
        public float CameraMoveDistance;
        public fixed int CacheLinePadding[15 - kCount];
    }

    /// <summary>
    /// Renders all Entities containing both RenderMesh & LocalToWorld components.
    /// </summary>
    [ExecuteAlways]
    //@TODO: Necessary due to empty component group. When Component group and archetype chunks are unified this should be removed
    [AlwaysUpdateSystem]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(LodRequirementsUpdateSystem))]
    public class RenderMeshSystemV2 : ComponentSystem
    {
        int m_LastFrozenChunksOrderVersion = -1;

        EntityQuery m_FrozenGroup;
        EntityQuery m_DynamicGroup;

        EntityQuery m_CullingJobDependencyGroup;
        InstancedRenderMeshBatchGroup m_InstancedRenderMeshBatchGroup;

        NativeHashMap<FrozenRenderSceneTag, int> m_SubsceneTagVersion;
        NativeList<SubSceneTagOrderVersion> m_LastKnownSubsceneTagVersion;

        #if UNITY_EDITOR
        EditorRenderData m_DefaultEditorRenderData = new EditorRenderData { SceneCullingMask = UnityEditor.SceneManagement.EditorSceneManager.DefaultSceneCullingMask };
        #else
        EditorRenderData m_DefaultEditorRenderData = new EditorRenderData { SceneCullingMask = ~0UL };
        #endif

        protected override void OnCreate()
        {
            //@TODO: Support SetFilter with EntityQueryDesc syntax

            m_FrozenGroup = GetEntityQuery(
                ComponentType.ChunkComponentReadOnly<ChunkWorldRenderBounds>(),
                ComponentType.ReadOnly<WorldRenderBounds>(),
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadOnly<RenderMesh>(),
                ComponentType.ReadOnly<FrozenRenderSceneTag>()
            );
            m_DynamicGroup = GetEntityQuery(
                ComponentType.ChunkComponentReadOnly<ChunkWorldRenderBounds>(),
                ComponentType.Exclude<FrozenRenderSceneTag>(),
                ComponentType.ReadOnly<WorldRenderBounds>(),
                ComponentType.ReadOnly<LocalToWorld>(),
                ComponentType.ReadOnly<RenderMesh>()
            );

            // This component group must include all types that are being used by the culling job
            m_CullingJobDependencyGroup = GetEntityQuery(
                ComponentType.ChunkComponentReadOnly<ChunkWorldRenderBounds>(),
                ComponentType.ReadOnly<RootLodRequirement>(),
                ComponentType.ReadOnly<LodRequirement>(),
                ComponentType.ReadOnly<WorldRenderBounds>()
            );

            m_InstancedRenderMeshBatchGroup = new InstancedRenderMeshBatchGroup(EntityManager, this, m_CullingJobDependencyGroup);
            m_SubsceneTagVersion = new NativeHashMap<FrozenRenderSceneTag, int>(1000,Allocator.Persistent);
            m_LastKnownSubsceneTagVersion = new NativeList<SubSceneTagOrderVersion>(Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            m_InstancedRenderMeshBatchGroup.CompleteJobs();
            m_InstancedRenderMeshBatchGroup.Dispose();
            m_SubsceneTagVersion.Dispose();
            m_LastKnownSubsceneTagVersion.Dispose();
        }

        public void CacheMeshBatchRendererGroup(FrozenRenderSceneTag tag, NativeArray<ArchetypeChunk> chunks, int chunkCount)
        {
            var RenderMeshType = GetArchetypeChunkSharedComponentType<RenderMesh>();
            var meshInstanceFlippedTagType = GetArchetypeChunkComponentType<RenderMeshFlippedWindingTag>();
            var editorRenderDataType = GetArchetypeChunkSharedComponentType<EditorRenderData>();

            Profiler.BeginSample("Sort Shared Renderers");
            var chunkRenderer = new NativeArray<int>(chunkCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            var sortedChunks = new NativeArraySharedValues<int>(chunkRenderer, Allocator.TempJob);

            var gatherChunkRenderersJob = new GatherChunkRenderers
            {
                Chunks = chunks,
                RenderMeshType = RenderMeshType,
                ChunkRenderer = chunkRenderer
            };
            var gatherChunkRenderersJobHandle = gatherChunkRenderersJob.Schedule(chunkCount, 64);
            var sortedChunksJobHandle = sortedChunks.Schedule(gatherChunkRenderersJobHandle);
            sortedChunksJobHandle.Complete();
            Profiler.EndSample();

            var sharedRenderCount = sortedChunks.SharedValueCount;
            var sharedRendererCounts = sortedChunks.GetSharedValueIndexCountArray();
            var sortedChunkIndices = sortedChunks.GetSortedIndices();

            m_InstancedRenderMeshBatchGroup.BeginBatchGroup();
            Profiler.BeginSample("Add New Batches");
            {
                var sortedChunkIndex = 0;
                for (int i = 0; i < sharedRenderCount; i++)
                {
                    var startSortedChunkIndex = sortedChunkIndex;
                    var endSortedChunkIndex = startSortedChunkIndex + sharedRendererCounts[i];

                    while (sortedChunkIndex < endSortedChunkIndex)
                    {
                        var chunkIndex = sortedChunkIndices[sortedChunkIndex];
                        var chunk = chunks[chunkIndex];
                        var rendererSharedComponentIndex = chunk.GetSharedComponentIndex(RenderMeshType);

                        var editorRenderDataIndex = chunk.GetSharedComponentIndex(editorRenderDataType);
                        var editorRenderData = m_DefaultEditorRenderData;
                        if (editorRenderDataIndex != -1)
                            editorRenderData = EntityManager.GetSharedComponentData<EditorRenderData>(editorRenderDataIndex);

                        var remainingEntitySlots = 1023;
                        var flippedWinding = chunk.Has(meshInstanceFlippedTagType);
                        int instanceCount = chunk.Count;
                        int startSortedIndex = sortedChunkIndex;
                        int batchChunkCount = 1;

                        remainingEntitySlots -= chunk.Count;
                        sortedChunkIndex++;

                        while (remainingEntitySlots > 0)
                        {
                            if (sortedChunkIndex >= endSortedChunkIndex)
                                break;

                            var nextChunkIndex = sortedChunkIndices[sortedChunkIndex];
                            var nextChunk = chunks[nextChunkIndex];
                            if (nextChunk.Count > remainingEntitySlots)
                                break;

                            var nextFlippedWinding = nextChunk.Has(meshInstanceFlippedTagType);
                            if (nextFlippedWinding != flippedWinding)
                                break;

#if UNITY_EDITOR
                            if (editorRenderDataIndex != nextChunk.GetSharedComponentIndex(editorRenderDataType))
                                break;
#endif

                            remainingEntitySlots -= nextChunk.Count;
                            instanceCount += nextChunk.Count;
                            batchChunkCount++;
                            sortedChunkIndex++;
                        }

                        m_InstancedRenderMeshBatchGroup.AddBatch(tag, rendererSharedComponentIndex, instanceCount, chunks, sortedChunkIndices, startSortedIndex, batchChunkCount, flippedWinding, editorRenderData);
                    }
                }
            }
            Profiler.EndSample();
            m_InstancedRenderMeshBatchGroup.EndBatchGroup(tag, chunks, sortedChunkIndices);

            chunkRenderer.Dispose();
            sortedChunks.Dispose();
        }

        void UpdateFrozenRenderBatches()
        {
            var staticChunksOrderVersion = EntityManager.GetComponentOrderVersion<FrozenRenderSceneTag>();
            if (staticChunksOrderVersion == m_LastFrozenChunksOrderVersion)
                return;

            for (int i = 0; i < m_LastKnownSubsceneTagVersion.Length; i++)
            {
                var scene = m_LastKnownSubsceneTagVersion[i].Scene;
                var version = m_LastKnownSubsceneTagVersion[i].Version;

                if (EntityManager.GetSharedComponentOrderVersion(scene) != version)
                {
                    // Debug.Log($"Removing scene:{scene:X8} batches");
                    Profiler.BeginSample("Remove Subscene");
                    m_SubsceneTagVersion.Remove(scene);
                    m_InstancedRenderMeshBatchGroup.RemoveTag(scene);
                    Profiler.EndSample();
                }
            }

            m_LastKnownSubsceneTagVersion.Clear();

            var loadedSceneTags = new List<FrozenRenderSceneTag>();
            EntityManager.GetAllUniqueSharedComponentData(loadedSceneTags);

            for (var i = 0; i < loadedSceneTags.Count; i++)
            {
                var subsceneTag = loadedSceneTags[i];
                int subsceneTagVersion = EntityManager.GetSharedComponentOrderVersion(subsceneTag);

                m_LastKnownSubsceneTagVersion.Add(new SubSceneTagOrderVersion
                {
                    Scene = subsceneTag,
                    Version = subsceneTagVersion
                });

                var alreadyTrackingSubscene = m_SubsceneTagVersion.TryGetValue(subsceneTag, out var _);
                if (alreadyTrackingSubscene)
                    continue;

                m_FrozenGroup.SetFilter(subsceneTag);

                var filteredChunks = m_FrozenGroup.CreateArchetypeChunkArray(Allocator.TempJob);

                m_FrozenGroup.ResetFilter();

                m_SubsceneTagVersion.TryAdd(subsceneTag, subsceneTagVersion);

                Profiler.BeginSample("CacheMeshBatchRenderGroup");
                CacheMeshBatchRendererGroup(subsceneTag, filteredChunks, filteredChunks.Length);
                Profiler.EndSample();

                filteredChunks.Dispose();
            }

            m_LastFrozenChunksOrderVersion = staticChunksOrderVersion;
        }

        void UpdateDynamicRenderBatches()
        {
            m_InstancedRenderMeshBatchGroup.RemoveTag(new FrozenRenderSceneTag());

            Profiler.BeginSample("CreateArchetypeChunkArray");
            var chunks = m_DynamicGroup.CreateArchetypeChunkArray(Allocator.TempJob);
            Profiler.EndSample();

            if (chunks.Length > 0)
            {
                CacheMeshBatchRendererGroup(new FrozenRenderSceneTag(), chunks, chunks.Length);
            }
            chunks.Dispose();
        }


        protected override void OnUpdate()
        {
            m_InstancedRenderMeshBatchGroup.CompleteJobs();
            m_InstancedRenderMeshBatchGroup.ResetLod();

            Profiler.BeginSample("UpdateFrozenRenderBatches");
            UpdateFrozenRenderBatches();
            Profiler.EndSample();

            Profiler.BeginSample("UpdateDynamicRenderBatches");
            UpdateDynamicRenderBatches();
            Profiler.EndSample();

            m_InstancedRenderMeshBatchGroup.LastUpdatedOrderVersion = EntityManager.GetComponentOrderVersion<RenderMesh>();
        }

#if UNITY_EDITOR
        public CullingStats ComputeCullingStats() { return m_InstancedRenderMeshBatchGroup.ComputeCullingStats(); }
#endif
    }
}
