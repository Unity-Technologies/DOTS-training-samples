using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

/*
 * Batch-oriented culling.
 *
 * This culling approach oriented from Megacity and works well for relatively
 * slow-moving cameras in a large, dense environment.
 *
 * The primary CPU costs involved in culling all the chunks of mesh instances
 * in megacity is touching the chunks of memory. A naive culling approach would
 * look like this:
 *
 *     for each chunk:
 *       select what instances should be enabled based on camera position (lod selection)
 *
 *     for each frustum:
 *       for each chunk:
 *         if the chunk is completely out of the frustum:
 *           discard
 *         else:
 *           for each instance in the chunk:
 *             if the instance is inside the frustum:
 *               write index of instance to output index buffer
 *
 * The approach implemented here does essentially this, but has been optimized
 * so that chunks need to be accessed as infrequently as possible:
 *
 * - Because the chunks are static, we can cache bounds information outside the chunks
 *
 * - Because the camera moves relatively slowly, we can compute a grace
 *   distance which the camera has to move (in any direction) before the LOD
 *   selection would compute a different result
 *
 * - Because only a some chunks straddle the frustum boundaries, we can treat
 *   them as "in" rather than "partial" to save touching their chunk memory
 *
 *
 * The code below is complicated by the fact that we maintain two indexing schemes.
 *
 * The external indices are the C++ batch renderer's idea of a batch. A batch
 * can contain up to 1023 model instances. This index set changes when batches
 * are removed, and these external indices are swapped from the end to maintain
 * a packed index set. The culling code here needs to maintain these external
 * batch indices only to communicate to the downstream renderer.
 *
 * Furthermore, we keep an internal index range. This is so that we have stable
 * indices that don't change as batches are removed. Because they are stable we
 * can use them as hash table indices and store information related to them freely.
 *
 * The core data organization is around this internal index space.
 *
 * We map from 1 internal index to N chunks. Each chunk directly corresponds to
 * an ECS chunk of instances to be culled and rendered.
 *
 * The chunk data tracks the bounds and some other bits of information that would
 * be expensive to reacquire from the chunk data itself.
 */
namespace Unity.Rendering
{
    unsafe struct ChunkInstanceLodEnabled
    {
        public fixed ulong Enabled[2];
    }

    internal struct LocalGroupKey : IEquatable<LocalGroupKey>
    {
        public int Value;

        public bool Equals(LocalGroupKey other)
        {
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value * 13317;
        }
    }

    internal struct Fixed16CamDistance
    {
        public const float kRes = 100.0f;

        public static ushort FromFloatCeil(float f)
        {
            return (ushort) math.clamp((int) math.ceil(f * kRes), 0, 0xffff);
        }

        public static ushort FromFloatFloor(float f)
        {
            return (ushort) math.clamp((int) math.floor(f * kRes), 0, 0xffff);
        }
    }

    [BurstCompile]
    unsafe struct SelectLodEnabled : IJobNativeMultiHashMapVisitKeyMutableValue<LocalGroupKey, BatchChunkData>
    {
        [ReadOnly] public LODGroupExtensions.LODParams LODParams;
        [ReadOnly] public NativeArray<byte> ForceLowLOD;
        [ReadOnly] public ArchetypeChunkComponentType<RootLodRequirement> RootLodRequirements;
        [ReadOnly] public ArchetypeChunkComponentType<LodRequirement> InstanceLodRequirements;
        public ushort CameraMoveDistanceFixed16;
        public float DistanceScale;
        public bool DistanceScaleChanged;

#if UNITY_EDITOR
        [NativeDisableUnsafePtrRestriction]
        public CullingStats* Stats;

#pragma warning disable 649
        [NativeSetThreadIndex]
        public int ThreadIndex;
#pragma warning restore 649

#endif

        public void ExecuteNext(LocalGroupKey internalBatchIndex, ref BatchChunkData chunkData)
        {
#if UNITY_EDITOR
            Stats[ThreadIndex].Stats[CullingStats.kLodTotal]++;
#endif
            var localIndex = internalBatchIndex.Value;
            var chunkInstanceCount = chunkData.ChunkInstanceCount;
            var isOrtho = LODParams.isOrtho;

            ChunkInstanceLodEnabled chunkEntityLodEnabled = chunkData.InstanceLodEnableds;
#if UNITY_EDITOR
            ChunkInstanceLodEnabled oldEntityLodEnabled = chunkEntityLodEnabled;
#endif
            var forceLowLOD = ForceLowLOD[localIndex];

            if (0 == (chunkData.Flags & BatchChunkData.kFlagHasLodData))
            {
#if UNITY_EDITOR
                Stats[ThreadIndex].Stats[CullingStats.kLodNoRequirements]++;
#endif
                chunkEntityLodEnabled.Enabled[0] = 0;
                chunkEntityLodEnabled.Enabled[1] = 0;
                chunkData.ForceLowLODPrevious = forceLowLOD;

                for (int i = 0; i < chunkInstanceCount; ++i)
                {
                    int wordIndex = i >> 6;
                    int bitIndex = i & 63;
                    chunkEntityLodEnabled.Enabled[wordIndex] |= 1ul << bitIndex;
                }
            }
            else
            {
                int diff = (int) chunkData.MovementGraceFixed16 - CameraMoveDistanceFixed16;
                chunkData.MovementGraceFixed16 = (ushort) math.max(0, diff);

                var graceExpired = chunkData.MovementGraceFixed16 == 0;
                var forceLodChanged = forceLowLOD != chunkData.ForceLowLODPrevious;

                if (graceExpired || forceLodChanged || DistanceScaleChanged)
                {
                    chunkEntityLodEnabled.Enabled[0] = 0;
                    chunkEntityLodEnabled.Enabled[1] = 0;

#if UNITY_EDITOR
                    Stats[ThreadIndex].Stats[CullingStats.kLodChunksTested]++;
#endif
                    var chunk = chunkData.Chunk;

                    var rootLodRequirements = chunk.GetNativeArray(RootLodRequirements);
                    var instanceLodRequirements = chunk.GetNativeArray(InstanceLodRequirements);

                    var chunkInstanceIndex = 0;
                    var rootIndex = 0;
                    float graceDistance = float.MaxValue;

                    while (chunkInstanceIndex < chunkInstanceCount)
                    {
                        var rootLodRequirement = rootLodRequirements[rootIndex];
                        var rootInstanceCount = rootLodRequirement.InstanceCount;

                        var rootLodDistance = math.select(DistanceScale * math.length(LODParams.cameraPos - rootLodRequirement.LOD.WorldReferencePosition), DistanceScale, isOrtho);

                        float rootMinDist = math.select(rootLodRequirement.LOD.MinDist, 0.0f, forceLowLOD == 1);
                        float rootMaxDist = rootLodRequirement.LOD.MaxDist;

                        graceDistance = math.min(math.abs(rootLodDistance - rootMinDist), graceDistance);
                        graceDistance = math.min(math.abs(rootLodDistance - rootMaxDist), graceDistance);

                        var rootLodIntersect = (rootLodDistance < rootMaxDist) && (rootLodDistance >= rootMinDist);

                        if (rootLodIntersect)
                        {
                            for (int i = 0; i < rootInstanceCount; i++)
                            {
                                var instanceLodRequirement = instanceLodRequirements[chunkInstanceIndex + i];
                                var instanceDistance = math.select(DistanceScale * math.length(LODParams.cameraPos - instanceLodRequirement.WorldReferencePosition), DistanceScale, isOrtho);

                                var instanceLodIntersect = (instanceDistance < instanceLodRequirement.MaxDist) && (instanceDistance >= instanceLodRequirement.MinDist);

                                graceDistance = math.min(math.abs(instanceDistance - instanceLodRequirement.MinDist), graceDistance);
                                graceDistance = math.min(math.abs(instanceDistance - instanceLodRequirement.MaxDist), graceDistance);
                                
                                if (instanceLodIntersect)
                                {
                                    var index = chunkInstanceIndex + i;
                                    var wordIndex = index >> 6;
                                    var bitIndex = index & 0x3f;
                                    var lodWord = chunkEntityLodEnabled.Enabled[wordIndex];

                                    lodWord |= 1UL << bitIndex;
                                    chunkEntityLodEnabled.Enabled[wordIndex] = lodWord;
                                }
                            }
                        }

                        chunkInstanceIndex += rootInstanceCount;
                        rootIndex++;
                    }

                    chunkData.MovementGraceFixed16 = Fixed16CamDistance.FromFloatFloor(graceDistance);
                    chunkData.ForceLowLODPrevious = forceLowLOD;
                }
            }


#if UNITY_EDITOR
            if (oldEntityLodEnabled.Enabled[0] != chunkEntityLodEnabled.Enabled[0] || oldEntityLodEnabled.Enabled[1] != chunkEntityLodEnabled.Enabled[1])
            {
                Stats[ThreadIndex].Stats[CullingStats.kLodChanged]++;
            }
#endif
            chunkData.InstanceLodEnableds = chunkEntityLodEnabled;
        }
    }

    public struct BatchCullingState
    {
        public int OutputCount;
    }

    [BurstCompile]
    unsafe struct SimpleCullingJob : IJobNativeMultiHashMapVisitKeyValue<LocalGroupKey, BatchChunkData>
    {
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<FrustumPlanes.PlanePacket4> Planes;
        [DeallocateOnJobCompletion] [NativeDisableParallelForRestriction] public NativeArray<BatchCullingState> BatchCullingStates;
        [ReadOnly] public NativeArray<int> InternalToExternalRemappingTable;

        [ReadOnly] public ArchetypeChunkComponentType<WorldRenderBounds> BoundsComponent;

        [NativeDisableParallelForRestriction] public NativeArray<int> IndexList;
        public NativeArray<BatchVisibility> Batches;

#if UNITY_EDITOR
        [NativeDisableUnsafePtrRestriction]
        public CullingStats* Stats;
        #pragma warning disable 649
        [NativeSetThreadIndex]
        public int ThreadIndex;
        #pragma warning restore 649
#endif

        public void ExecuteNext(LocalGroupKey internalBatchIndex, BatchChunkData chunkData)
        {
#if UNITY_EDITOR
            ref var stats = ref Stats[ThreadIndex];
            stats.Stats[CullingStats.kChunkTotal]++;
#endif

            var externalBatchIndex = InternalToExternalRemappingTable[internalBatchIndex.Value];

            var batch = Batches[externalBatchIndex];

            var inState = BatchCullingStates[internalBatchIndex.Value];

            int batchOutputOffset = batch.offset;
            int batchOutputCount = inState.OutputCount;
            int processedInstanceCount = chunkData.BatchOffset;

            var chunkInstanceCount = chunkData.ChunkInstanceCount;
            var chunkEntityLodEnabled = chunkData.InstanceLodEnableds;
            var anyLodEnabled = (chunkEntityLodEnabled.Enabled[0] | chunkEntityLodEnabled.Enabled[1]) != 0;

            if (anyLodEnabled)
            {
#if UNITY_EDITOR
                stats.Stats[CullingStats.kChunkCountAnyLod]++;
#endif

                var chunkBounds = chunkData.ChunkBounds;

                var perInstanceCull = 0 != (chunkData.Flags & BatchChunkData.kFlagInstanceCulling);

                var chunkIn = perInstanceCull ?
                    FrustumPlanes.Intersect2(Planes, chunkBounds.Value) :
                    FrustumPlanes.Intersect2NoPartial(Planes, chunkBounds.Value);

                if (chunkIn == FrustumPlanes.IntersectResult.Partial)
                {
#if UNITY_EDITOR
                    int instanceTestCount = 0;
#endif

                    var chunk = chunkData.Chunk;
                    var chunkInstanceBounds = chunk.GetNativeArray(BoundsComponent);

                    for (int j = 0; j < 2; j++)
                    {
                        var lodWord = chunkEntityLodEnabled.Enabled[j];

                        while (lodWord != 0)
                        {
                            var bitIndex = math.tzcnt(lodWord);
                            var finalIndex = (j << 6) + bitIndex;

                            IndexList[batchOutputOffset + batchOutputCount] = processedInstanceCount + finalIndex;

                            int advance = FrustumPlanes.Intersect2(Planes, chunkInstanceBounds[finalIndex].Value) != FrustumPlanes.IntersectResult.Out ? 1 : 0;
                            batchOutputCount += advance;

                            lodWord ^= 1ul << bitIndex;

#if UNITY_EDITOR
                            instanceTestCount++;
#endif
                        }
                    }

#if UNITY_EDITOR
                    stats.Stats[CullingStats.kChunkCountInstancesProcessed]++;
                    stats.Stats[CullingStats.kInstanceTests] += instanceTestCount;
#endif
                }
                else if (chunkIn == FrustumPlanes.IntersectResult.In)
                {
#if UNITY_EDITOR
                    stats.Stats[CullingStats.kChunkCountFullyIn]++;
#endif

                    for (int j = 0; j < 2; j++)
                    {
                        var lodWord = chunkEntityLodEnabled.Enabled[j];

                        while (lodWord != 0)
                        {
                            var bitIndex = math.tzcnt(lodWord);
                            var finalIndex = (j << 6) + bitIndex;
                            IndexList[batchOutputOffset + batchOutputCount] = processedInstanceCount + finalIndex;
                            batchOutputCount += 1;
                            lodWord ^= 1ul << bitIndex;
                        }
                    }
                }
            }

            var outState = inState;
            outState.OutputCount = batchOutputCount;
            BatchCullingStates[internalBatchIndex.Value] = outState;

            batch.visibleCount = batchOutputCount;
            Batches[externalBatchIndex] = batch;
        }
    }

    internal struct BatchChunkData
    {
        public const int kFlagHasLodData = 1 << 0;
        public const int kFlagInstanceCulling = 1 << 1;
                                                               // size  // start - end offset
        public short ChunkInstanceCount;                       //  2     0 - 2
        public short BatchOffset;                              //  2     2 - 4
        public ushort MovementGraceFixed16;                    //  2     4 - 6
        public byte Flags;                                     //  1     6 - 7
        public byte ForceLowLODPrevious;                       //  1     7 - 8
        public ChunkWorldRenderBounds ChunkBounds;             // 24     8 - 32
        public ChunkInstanceLodEnabled InstanceLodEnableds;    // 16     32 - 48
        public ArchetypeChunk Chunk;                           //  8     48 - 64
    }

#if HYBRID_INSTANCE_DEFAULT_DATA_2019_3_B6       // New InstancedRenderMeshBatchGroup code. Compatible with 2019.3.0b6 and newer.

    public unsafe class InstancedRenderMeshBatchGroup
    {
        const int kMaxBatchCount = 64 * 1024;
        const int kMaxArchetypeProperties = 256;

        EntityManager m_EntityManager;
        ComponentSystemBase m_ComponentSystem;
        JobHandle m_CullingJobDependency;
        JobHandle m_LODDependency;
        EntityQuery m_CullingJobDependencyGroup;
        BatchRendererGroup m_BatchRendererGroup;

        // Our idea of batches. This is indexed by local batch indices.
        NativeMultiHashMap<LocalGroupKey, BatchChunkData> m_BatchToChunkMap;

        // Maps from internal to external batch ids
        NativeArray<int> m_InternalToExternalIds;
        NativeArray<int> m_ExternalToInternalIds;

        // These arrays are parallel and allocated up to kMatchBatchCount. They are indexed by local batch indices.
        NativeArray<FrozenRenderSceneTag> m_Tags;
        NativeArray<byte> m_ForceLowLOD;

        // Tracks the highest index (+1) in use across InstanceCounts/Tags/LodSkip.
        int m_InternalBatchRange;
        int m_ExternalBatchCount;

        // Per-batch material properties
        List<MaterialPropertyBlock> m_MaterialPropertyBlocks;

        // This is a hack to allocate local batch indices in response to external batches coming and going
        int m_LocalIdCapacity;
        NativeArray<int> m_LocalIdPool;

        public int LastUpdatedOrderVersion = -1;

#if UNITY_EDITOR
        float m_CamMoveDistance;
#endif

#if UNITY_EDITOR
        private CullingStats* m_CullingStats = null;

        public CullingStats ComputeCullingStats()
        {
            var result = default(CullingStats);
            for (int i = 0; i < JobsUtility.MaxJobThreadCount; ++i)
            {
                ref var s = ref m_CullingStats[i];

                for (int f = 0; f < (int)CullingStats.kCount; ++f)
                {
                    result.Stats[f] += s.Stats[f];
                }
            }
            result.CameraMoveDistance = m_CamMoveDistance;
            return result;
        }
#endif

        private bool m_ResetLod;

        LODGroupExtensions.LODParams m_PrevLODParams;
        float3 m_PrevCameraPos;
        float m_PrevLodDistanceScale;

        ProfilerMarker m_RemoveBatchMarker;

        struct MaterialPropertyType
        {
            public int nameId;
            public int nameIdArray;
            public int typeIndex;
            public MaterialPropertyFormat format;
            public int numFormatComponents;
        };

        struct MaterialPropertyPointer
        {
            public float* ptr;
            public ArchetypeChunkComponentTypeDynamic type;
            public int numFormatComponents;
        };

        List<MaterialPropertyType> m_MaterialPropertyTypes;
        MaterialPropertyPointer[] m_MaterialPropertyPointers;

        public InstancedRenderMeshBatchGroup(EntityManager entityManager, ComponentSystemBase componentSystem, EntityQuery cullingJobDependencyGroup)
        {
            m_BatchRendererGroup = new BatchRendererGroup(this.OnPerformCulling);
            m_EntityManager = entityManager;
            m_ComponentSystem = componentSystem;
            m_CullingJobDependencyGroup = cullingJobDependencyGroup;
            m_BatchToChunkMap = new NativeMultiHashMap<LocalGroupKey, BatchChunkData>(32, Allocator.Persistent);
            m_LocalIdPool = new NativeArray<int>(kMaxBatchCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_Tags = new NativeArray<FrozenRenderSceneTag>(kMaxBatchCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_ForceLowLOD = new NativeArray<byte>(kMaxBatchCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_InternalToExternalIds = new NativeArray<int>(kMaxBatchCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_ExternalToInternalIds = new NativeArray<int>(kMaxBatchCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_ResetLod = true;
            m_InternalBatchRange = 0;
            m_ExternalBatchCount = 0;

            m_RemoveBatchMarker = new ProfilerMarker("BatchRendererGroup.Remove");

#if UNITY_EDITOR
            m_CullingStats = (CullingStats*)UnsafeUtility.Malloc(JobsUtility.MaxJobThreadCount * sizeof(CullingStats), 64, Allocator.Persistent);
#endif
            m_MaterialPropertyBlocks = new List<MaterialPropertyBlock>();

            // Collect all components with [MaterialProperty] attribute
            m_MaterialPropertyTypes = new List<MaterialPropertyType>();
            foreach (var typeInfo in TypeManager.AllTypes)
            {
                var type = typeInfo.Type;
                if (typeof(IComponentData).IsAssignableFrom(type))
                { 
                    var attributes = type.GetCustomAttributes(typeof(MaterialPropertyAttribute), false);
                    if (attributes.Length > 0)
                    { 
                        var format = ((MaterialPropertyAttribute)attributes[0]).Format;
                        int numFormatComponents = 1;
                        switch (format)
                        {
                            case MaterialPropertyFormat.Float: numFormatComponents = 1; break;
                            case MaterialPropertyFormat.Float4: numFormatComponents = 4; break;
                            case MaterialPropertyFormat.Float4x4: numFormatComponents = 16; break;
                        }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                        if (UnsafeUtility.SizeOf(type) != numFormatComponents * sizeof(float))
                        {
                            throw new InvalidOperationException($"Material property component {type} (size = {UnsafeUtility.SizeOf(type)}) cannot be reinterpreted as {numFormatComponents} floats (size = {numFormatComponents * sizeof(float)}). Sizes must match.");
                        }
#endif

                        m_MaterialPropertyTypes.Add(new MaterialPropertyType { typeIndex = TypeManager.GetTypeIndex(type), nameId = Shader.PropertyToID(((MaterialPropertyAttribute)attributes[0]).Name), nameIdArray = Shader.PropertyToID(((MaterialPropertyAttribute)attributes[0]).Name + "_Array"), format = format, numFormatComponents = numFormatComponents });
                    }
                }
            }

            m_MaterialPropertyPointers = new MaterialPropertyPointer[m_MaterialPropertyTypes.Count];

            ResetLocalIdPool();
        }

        private void ResetLocalIdPool()
        {
            m_LocalIdCapacity = kMaxBatchCount;
            for (int i = 0; i < kMaxBatchCount; ++i)
            {
                m_LocalIdPool[i] = kMaxBatchCount - i - 1;
            }
        }

        public void Dispose()
        {
#if UNITY_EDITOR
            UnsafeUtility.Free(m_CullingStats, Allocator.Persistent);

            m_CullingStats = null;
#endif
            m_LocalIdPool.Dispose();
            m_ExternalToInternalIds.Dispose();
            m_InternalToExternalIds.Dispose();
            m_BatchRendererGroup.Dispose();
            m_BatchToChunkMap.Dispose();
            m_Tags.Dispose();
            m_ForceLowLOD.Dispose();
            m_ResetLod = true;
            m_InternalBatchRange = 0;
            m_ExternalBatchCount = 0;
            m_MaterialPropertyBlocks.Clear();
        }

        public void Clear()
        {
            m_BatchRendererGroup.Dispose();
            m_BatchRendererGroup = new BatchRendererGroup(this.OnPerformCulling);
            m_PrevLODParams = new LODGroupExtensions.LODParams();
            m_PrevCameraPos = default(float3);
            m_PrevLodDistanceScale = 0.0f;
            m_ResetLod = true;
            m_InternalBatchRange = 0;
            m_ExternalBatchCount = 0;

            m_BatchToChunkMap.Clear();
            m_MaterialPropertyBlocks.Clear();

            ResetLocalIdPool();
        }

        public int AllocLocalId()
        {
            Assert.IsTrue(m_LocalIdCapacity > 0);
            int result = m_LocalIdPool[m_LocalIdCapacity - 1];
            --m_LocalIdCapacity;
            return result;
        }

        public void FreeLocalId(int id)
        {
            Assert.IsTrue(m_LocalIdCapacity < kMaxBatchCount);
            int result = m_LocalIdPool[m_LocalIdCapacity] = id;
            ++m_LocalIdCapacity;
        }

        public void ResetLod()
        {
            m_PrevLODParams = new LODGroupExtensions.LODParams();
            m_ResetLod = true;
        }

        public unsafe JobHandle OnPerformCulling(BatchRendererGroup rendererGroup, BatchCullingContext cullingContext)
        {
#if false
            // Reset all visible counts to 0 - to not crash Unity when there is bugs in this code during dev.
            for (int i = 0; i < cullingContext.batchVisibility.Length; ++i)
            {
                var v = cullingContext.batchVisibility[i];
                v.visibleCount = 0;
                cullingContext.batchVisibility[i] = v;
            }
#endif

            if (LastUpdatedOrderVersion != m_EntityManager.GetComponentOrderVersion<RenderMesh>())
            {
                // Debug.LogError("The chunk layout of RenderMesh components has changed between updating and culling. This is not allowed, rendering is disabled.");
                return default(JobHandle);
            }

            var batchCount = cullingContext.batchVisibility.Length;
            if (batchCount == 0)
                return new JobHandle();;

            var lodParams = LODGroupExtensions.CalculateLODParams(cullingContext.lodParameters);

            Profiler.BeginSample("OnPerformCulling");

            int cullingPlaneCount = cullingContext.cullingPlanes.Length;
            int packetCount = (cullingPlaneCount + 3 )>> 2;
            var planes = FrustumPlanes.BuildSOAPlanePackets(cullingContext.cullingPlanes, Allocator.TempJob);

            bool singleThreaded = false;

            JobHandle cullingDependency;
            var resetLod = m_ResetLod || (!lodParams.Equals(m_PrevLODParams));
            if (resetLod)
            {
                // Depend on all component ata we access + previous jobs since we are writing to a single
                // m_ChunkInstanceLodEnableds array.
                var lodJobDependency = JobHandle.CombineDependencies(m_CullingJobDependency, m_CullingJobDependencyGroup.GetDependency());

                float cameraMoveDistance = math.length(m_PrevCameraPos - lodParams.cameraPos);
                var lodDistanceScaleChanged = lodParams.distanceScale != m_PrevLodDistanceScale;

#if UNITY_EDITOR
                // Record this separately in the editor for stats display
                m_CamMoveDistance = cameraMoveDistance;
#endif

                var selectLodEnabledJob = new SelectLodEnabled
                {
                    ForceLowLOD = m_ForceLowLOD,
                    LODParams = lodParams,
                    RootLodRequirements = m_ComponentSystem.GetArchetypeChunkComponentType<RootLodRequirement>(true),
                    InstanceLodRequirements = m_ComponentSystem.GetArchetypeChunkComponentType<LodRequirement>(true),
                    CameraMoveDistanceFixed16 = Fixed16CamDistance.FromFloatCeil(cameraMoveDistance * lodParams.distanceScale),
                    DistanceScale = lodParams.distanceScale,
                    DistanceScaleChanged = lodDistanceScaleChanged,
#if UNITY_EDITOR
                    Stats = m_CullingStats,
#endif
                };

                cullingDependency = m_LODDependency = selectLodEnabledJob.Schedule(m_BatchToChunkMap, singleThreaded ? 150000 : m_BatchToChunkMap.Capacity / 64, lodJobDependency);

                m_PrevLODParams = lodParams;
                m_PrevLodDistanceScale = lodParams.distanceScale;
                m_PrevCameraPos = lodParams.cameraPos;
                m_ResetLod = false;
#if UNITY_EDITOR
                UnsafeUtility.MemClear(m_CullingStats, sizeof(CullingStats) * JobsUtility.MaxJobThreadCount);
#endif
            }
            else
            {
                // Depend on all component ata we access + previous m_LODDependency job
                cullingDependency = JobHandle.CombineDependencies(m_LODDependency, m_CullingJobDependencyGroup.GetDependency());
            }

            var batchCullingStates = new NativeArray<BatchCullingState>(m_InternalBatchRange, Allocator.TempJob, NativeArrayOptions.ClearMemory);

            var simpleCullingJob = new SimpleCullingJob
            {
                Planes = planes,
                BatchCullingStates = batchCullingStates,
                BoundsComponent = m_ComponentSystem.GetArchetypeChunkComponentType<WorldRenderBounds>(true),
                IndexList = cullingContext.visibleIndices,
                Batches = cullingContext.batchVisibility,
                InternalToExternalRemappingTable = m_InternalToExternalIds,
#if UNITY_EDITOR
                Stats = m_CullingStats,
#endif
            };

            var simpleCullingJobHandle = simpleCullingJob.Schedule(m_BatchToChunkMap, singleThreaded ? 150000 : 1024, cullingDependency);

            DidScheduleCullingJob(simpleCullingJobHandle);

            Profiler.EndSample();
            return simpleCullingJobHandle;
        }

        public void BeginBatchGroup()
        {

        }

        public unsafe void AddBatch(FrozenRenderSceneTag tag, int rendererSharedComponentIndex, int batchInstanceCount, NativeArray<ArchetypeChunk> chunks, NativeArray<int> sortedChunkIndices, int startSortedIndex, int chunkCount, bool flippedWinding, EditorRenderData data)
        {
            var bigBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(16738.0f, 16738.0f, 16738.0f));

            var rendererSharedComponent = m_EntityManager.GetSharedComponentData<RenderMesh>(rendererSharedComponentIndex);
            var mesh = rendererSharedComponent.mesh;
            var material = rendererSharedComponent.material;
            var castShadows = rendererSharedComponent.castShadows;
            var receiveShadows = rendererSharedComponent.receiveShadows;
            var subMeshIndex = rendererSharedComponent.subMesh;
            var layer = rendererSharedComponent.layer;

            if (mesh == null || material == null)
            {
                return;
            }

            Profiler.BeginSample("AddBatch");
            int externalBatchIndex = m_BatchRendererGroup.AddBatch(mesh, subMeshIndex, material, layer, castShadows, receiveShadows, flippedWinding, bigBounds, batchInstanceCount, null, data.PickableObject, data.SceneCullingMask);

            if (externalBatchIndex > m_MaterialPropertyBlocks.Count - 1)
                m_MaterialPropertyBlocks.Add(new MaterialPropertyBlock());
            
            var propertyBlock = m_MaterialPropertyBlocks[externalBatchIndex];
            m_BatchRendererGroup.SetInstancingData(externalBatchIndex, batchInstanceCount, propertyBlock);

            var matrices = (float4x4*) m_BatchRendererGroup.GetBatchMatrices(externalBatchIndex).GetUnsafePtr();
            Profiler.EndSample();

            int internalBatchIndex = AllocLocalId();
            //Debug.Log($"Adding internal index {internalBatchIndex} for external index {externalBatchIndex}; pre count {m_ExternalBatchCount}");

            m_ExternalToInternalIds[externalBatchIndex] = internalBatchIndex;
            m_InternalToExternalIds[internalBatchIndex] = externalBatchIndex;

            var boundsType = m_ComponentSystem.GetArchetypeChunkComponentType<ChunkWorldRenderBounds>(true);
            var localToWorldType = m_ComponentSystem.GetArchetypeChunkComponentType<LocalToWorld>(true);
            var rootLodRequirements = m_ComponentSystem.GetArchetypeChunkComponentType<RootLodRequirement>(true);
            var instanceLodRequirements = m_ComponentSystem.GetArchetypeChunkComponentType<LodRequirement>(true);
            var perInstanceCullingTag = m_ComponentSystem.GetArchetypeChunkComponentType<PerInstanceCullingTag>(true);

            var shader = material.shader;
            var numProperties = shader.GetPropertyCount();
            int numShaderMaterialProperties = 0;
            for (int i = 0; i < numProperties; ++i)
            {
                var flags = shader.GetPropertyFlags(i);
                if (((uint)flags & (uint)ShaderPropertyFlags.HideInInspector) == 0)
                {
                    //var nameId = Shader.PropertyToID(shader.GetPropertyName(i));      // This causes GCAlloc
                    var nameId = shader.GetPropertyNameId(i);                           // New C++ API (landed 3 hours later)

                    int materialPropertyIndex = 0;
                    for (; materialPropertyIndex < m_MaterialPropertyTypes.Count; ++materialPropertyIndex)
                    {
                        if (nameId == m_MaterialPropertyTypes[materialPropertyIndex].nameId) break;
                    }

                    // Found?
                    if (materialPropertyIndex < m_MaterialPropertyTypes.Count)
                    {
                        float* nativePtr = null;

                        switch (m_MaterialPropertyTypes[materialPropertyIndex].format)
                        {
                            case MaterialPropertyFormat.Float:
                                {
                                    var arr1 = m_BatchRendererGroup.GetBatchScalarArray(externalBatchIndex, m_MaterialPropertyTypes[materialPropertyIndex].nameIdArray);
                                    nativePtr = (float*)arr1.GetUnsafePtr();
                                    var defaultValue = material.GetFloat(m_MaterialPropertyTypes[materialPropertyIndex].nameId);
                                    UnsafeUtility.MemCpyReplicate(nativePtr, &defaultValue, UnsafeUtility.SizeOf<float>(), batchInstanceCount); // TODO: Reuse batches to avoid default initialization every frame.
                                }
                                break;

                            case MaterialPropertyFormat.Float4:
                                {
                                    var arr4 = m_BatchRendererGroup.GetBatchVectorArray(externalBatchIndex, m_MaterialPropertyTypes[materialPropertyIndex].nameIdArray);
                                    nativePtr = (float*)arr4.GetUnsafePtr();
                                    var defaultValue = material.GetVector(m_MaterialPropertyTypes[materialPropertyIndex].nameId);
                                    UnsafeUtility.MemCpyReplicate(nativePtr, &defaultValue, UnsafeUtility.SizeOf<float4>(), batchInstanceCount); // TODO: Reuse batches to avoid default initialization every frame.
                                }
                                break;

                            case MaterialPropertyFormat.Float4x4:
                                {
                                    var arr4x4 = m_BatchRendererGroup.GetBatchMatrixArray(externalBatchIndex, m_MaterialPropertyTypes[materialPropertyIndex].nameIdArray);
                                    nativePtr = (float*)arr4x4.GetUnsafePtr();
                                    var defaultValue = material.GetMatrix(m_MaterialPropertyTypes[materialPropertyIndex].nameId);
                                    UnsafeUtility.MemCpyReplicate(nativePtr, &defaultValue, UnsafeUtility.SizeOf<float4x4>(), batchInstanceCount); // TODO: Reuse batches to avoid default initialization every frame.
                                }
                                break;
                        }

                        m_MaterialPropertyPointers[numShaderMaterialProperties++] = new MaterialPropertyPointer 
                        { 
                            ptr = nativePtr,
                            type = m_ComponentSystem.GetArchetypeChunkComponentTypeDynamic(ComponentType.ReadOnly(m_MaterialPropertyTypes[materialPropertyIndex].typeIndex)),
                            numFormatComponents = m_MaterialPropertyTypes[materialPropertyIndex].numFormatComponents
                        };
                    }
                }

            }

            int runningOffset = 0;
            var previousArchetype = new EntityArchetype();
            int numActiveArchetypeMaterialProperties = 0;
            var archetypeActiveMaterialProperties = new NativeArray<int>(kMaxArchetypeProperties, Allocator.Temp, NativeArrayOptions.UninitializedMemory); 

            for (int i = 0; i < chunkCount; ++i)
            {
                var chunk = chunks[sortedChunkIndices[startSortedIndex + i]];
                var bounds = chunk.GetChunkComponentData(boundsType);

                var localKey = new LocalGroupKey { Value = internalBatchIndex };
                var hasLodData = chunk.Has(rootLodRequirements) && chunk.Has(instanceLodRequirements);
                var hasPerInstanceCulling = !hasLodData || chunk.Has(perInstanceCullingTag);

                Assert.IsTrue(chunk.Count <= 128);

                m_BatchToChunkMap.Add(localKey, new BatchChunkData
                {
                    Chunk = chunk,
                    Flags = (byte) ((hasLodData ? BatchChunkData.kFlagHasLodData : 0) | (hasPerInstanceCulling ? BatchChunkData.kFlagInstanceCulling : 0)),
                    ChunkBounds = bounds,
                    ChunkInstanceCount = (short) chunk.Count,
                    BatchOffset = (short) runningOffset,
                    InstanceLodEnableds = default
                });

                var matrixSizeOf = UnsafeUtility.SizeOf<float4x4>();
                var localToWorld = chunk.GetNativeArray(localToWorldType);
                float4x4* srcMatrices = (float4x4*) localToWorld.GetUnsafeReadOnlyPtr();

                UnsafeUtility.MemCpy(matrices, srcMatrices, matrixSizeOf * chunk.Count);

                matrices += chunk.Count;

                // Go though all [MaterialProperty] component types in the chunk, and collect them to an array. Use same components until archetype changes.
                if (previousArchetype != chunk.Archetype)
                {
                    previousArchetype = chunk.Archetype;
                    numActiveArchetypeMaterialProperties = 0;

                    for (int j = 0; j < numShaderMaterialProperties; j++)
                    {
                        if (chunk.Has(m_MaterialPropertyPointers[j].type))
                        {
                            archetypeActiveMaterialProperties[numActiveArchetypeMaterialProperties++] = j;
                        }
                    }
                }
               
                // Memcpy all material property instance data in [MaterialProperty] types to C++ side arrays
                for (int j = 0; j < numActiveArchetypeMaterialProperties; j++)
                {
                    var componentIndex = archetypeActiveMaterialProperties[j];
                    var componentSize = UnsafeUtility.SizeOf<float>() * m_MaterialPropertyPointers[componentIndex].numFormatComponents;
                    var chunkData = chunk.GetDynamicComponentDataArrayReinterpret<float>(m_MaterialPropertyPointers[componentIndex].type, componentSize);
                    Debug.Assert(chunkData.Length > 0);

                    float* srcData = (float*)chunkData.GetUnsafeReadOnlyPtr();
                    float* dstData = m_MaterialPropertyPointers[componentIndex].ptr + runningOffset * m_MaterialPropertyPointers[componentIndex].numFormatComponents;
                    int copySize = chunk.Count * componentSize;

                    UnsafeUtility.MemCpy(dstData, srcData, copySize);
                }

                runningOffset += chunk.Count;
            }

            archetypeActiveMaterialProperties.Dispose();

            m_Tags[internalBatchIndex] = tag;
            m_ForceLowLOD[internalBatchIndex] = (byte) ((tag.SectionIndex == 0 && tag.HasStreamedLOD != 0) ? 1 : 0);

            m_InternalBatchRange = math.max(m_InternalBatchRange, internalBatchIndex + 1);
            m_ExternalBatchCount = externalBatchIndex + 1;

            SanityCheck();
        }
        
        private void SanityCheck()
        {
#if false
            //Debug.Log($"SanityCheck ir {m_InternalBatchRange} ec {m_ExternalBatchCount}");
            Assert.IsTrue(m_InternalBatchRange >= m_ExternalBatchCount);

            var populated = 0;

            var lookup = new Dictionary<int, bool>();

            for (int i = 0; i < m_InternalBatchRange; ++i)
            {
                var internalId = i;
                var externalId = m_InternalToExternalIds[i];
                if (externalId == -1)
                    continue;
                if (externalId >= m_ExternalBatchCount)
                {
                    Debug.Log($"Invalid external id {externalId} for internal id {i} (max {m_ExternalBatchCount})");
                }
                else
                {
                    if (lookup.ContainsKey(externalId))
                    {
                        Debug.Log($"Duplicate mapping e={externalId} at internal id {i}");
                    }
                    else
                    {
                        lookup.Add(externalId, true);
                    }
                }
            }

            if (lookup.Count != m_ExternalBatchCount)
            {
                Debug.Log($"Unreachable external batches: have {lookup.Count} but need {m_ExternalBatchCount}");
            }

            lookup.Clear();

            for (int i = 0; i < m_ExternalBatchCount; ++i)
            {
                var externalId = i;
                var internalId = m_ExternalToInternalIds[i];
                if (internalId < 0 || internalId >= m_InternalBatchRange)
                {
                    Debug.Log($"Invalid internal id {internalId} for external id {externalId}");
                }
                else
                {
                    if (lookup.ContainsKey(internalId))
                    {
                        Debug.Log($"Duplicate mapping e={externalId} to internal id {internalId}");
                    }
                    else
                    {
                        lookup.Add(internalId, true);
                    }
                }

                var ext2 = m_InternalToExternalIds[internalId];
                if (ext2 != externalId)
                {
                    Debug.Log($"Invalid round trip for internal id {internalId} for external id {externalId}; got {ext2}");
                }
            }

            if (lookup.Count != m_ExternalBatchCount)
            {
                Debug.Log($"Bad count of internal batches: have {lookup.Count} but need {m_ExternalBatchCount}");
            }
#endif
        }

        public void EndBatchGroup(FrozenRenderSceneTag tag, NativeArray<ArchetypeChunk> chunks, NativeArray<int> sortedChunkIndices)
        {
            // Disable force low lod  based on loading a streaming zone
            if (tag.SectionIndex > 0 && tag.HasStreamedLOD != 0)
            {
                for (int i = 0; i < m_InternalBatchRange; i++)
                {
                    if (m_Tags[i].SceneGUID.Equals(tag.SceneGUID))
                    {
                        m_ForceLowLOD[i] = 0;
                    }
                }
            }
        }

        public void RemoveTag(FrozenRenderSceneTag tag)
        {
            // Enable force low lod based on the high lod being streamed out
            if (tag.SectionIndex > 0 && tag.HasStreamedLOD != 0)
            {
                for (int i = 0; i < m_InternalBatchRange; i++)
                {
                    if (m_Tags[i].SceneGUID.Equals(tag.SceneGUID))
                    {
                        m_ForceLowLOD[i] = 1;
                    }
                }
            }

            Profiler.BeginSample("RemoveTag");
            // Remove any tag that need to go
            for (int i = m_InternalBatchRange-1; i >= 0; i--)
            {
                var shouldRemove = m_Tags[i].Equals(tag);
                if (!shouldRemove)
                    continue;

                var externalBatchIndex = m_InternalToExternalIds[i];
                if (externalBatchIndex == -1)
                    continue;

                //Debug.Log($"Removing internal index {i} for external index {externalBatchIndex}; pre batch count = {m_ExternalBatchCount}");

                m_RemoveBatchMarker.Begin();
                m_BatchRendererGroup.RemoveBatch(externalBatchIndex);
                m_RemoveBatchMarker.End();

                // I->E: [ x: 0, y: 1, z: 2 ]  -> [ x: 0, y: ?, z: 2 ]
                // E->I: [ 0: x, 1: y, 2: z ]  -> [ 0: x, 1: z ]
                // B:    [ A B C ]             -> [ A C ]


                // Update remapping for external block. The render group will swap with the end, so replicate that behavior.
                var swappedInternalId = m_ExternalToInternalIds[m_ExternalBatchCount - 1];

                m_ExternalToInternalIds[externalBatchIndex] = swappedInternalId;
                m_InternalToExternalIds[swappedInternalId] = externalBatchIndex;

                // Return local id to pool
                FreeLocalId(i);

                // Invalidate id remapping table for this internal id
                m_InternalToExternalIds[i] = -1;

                m_Tags[i] = default(FrozenRenderSceneTag);

                var localKey = new LocalGroupKey { Value = i };
                m_BatchToChunkMap.Remove(localKey);

                m_ExternalBatchCount--;
            }

            Profiler.EndSample();

            SanityCheck();
        }

        public void CompleteJobs()
        {
            m_CullingJobDependency.Complete();
            m_CullingJobDependencyGroup.CompleteDependency();
        }


        void DidScheduleCullingJob(JobHandle job)
        {
            m_CullingJobDependency = JobHandle.CombineDependencies(job, m_CullingJobDependency);
            m_CullingJobDependencyGroup.AddDependency(job);
        }
    }

#else       // Old InstancedRenderMeshBatchGroup code. Compatible with all 2019.3 versions.

    public unsafe class InstancedRenderMeshBatchGroup
    {
        const int kMaxBatchCount = 64 * 1024;
        const int kMaxArchetypeProperties = 256;

        EntityManager m_EntityManager;
        ComponentSystemBase m_ComponentSystem;
        JobHandle m_CullingJobDependency;
        JobHandle m_LODDependency;
        EntityQuery m_CullingJobDependencyGroup;
        BatchRendererGroup m_BatchRendererGroup;

        // Our idea of batches. This is indexed by local batch indices.
        NativeMultiHashMap<LocalGroupKey, BatchChunkData> m_BatchToChunkMap;

        // Maps from internal to external batch ids
        NativeArray<int> m_InternalToExternalIds;
        NativeArray<int> m_ExternalToInternalIds;

        // These arrays are parallel and allocated up to kMatchBatchCount. They are indexed by local batch indices.
        NativeArray<FrozenRenderSceneTag> m_Tags;
        NativeArray<byte> m_ForceLowLOD;

        // Tracks the highest index (+1) in use across InstanceCounts/Tags/LodSkip.
        int m_InternalBatchRange;
        int m_ExternalBatchCount;

        // Per-batch material properties
        List<MaterialPropertyBlock> m_MaterialPropertyBlocks;

        // This is a hack to allocate local batch indices in response to external batches coming and going
        int m_LocalIdCapacity;
        NativeArray<int> m_LocalIdPool;

        public int LastUpdatedOrderVersion = -1;

#if UNITY_EDITOR
        float m_CamMoveDistance;
#endif

#if UNITY_EDITOR
        private CullingStats* m_CullingStats = null;

        public CullingStats ComputeCullingStats()
        {
            var result = default(CullingStats);
            for (int i = 0; i < JobsUtility.MaxJobThreadCount; ++i)
            {
                ref var s = ref m_CullingStats[i];

                for (int f = 0; f < (int)CullingStats.kCount; ++f)
                {
                    result.Stats[f] += s.Stats[f];
                }
            }
            result.CameraMoveDistance = m_CamMoveDistance;
            return result;
        }
#endif

        private bool m_ResetLod;

        LODGroupExtensions.LODParams m_PrevLODParams;
        float3 m_PrevCameraPos;
        float m_PrevLodDistanceScale;

        ProfilerMarker m_RemoveBatchMarker;

        struct MaterialPropertyType
        {
            public int nameId;
            public int typeIndex;
            public MaterialPropertyFormat format;
            public int numFormatComponents;
        };

        struct MaterialPropertyPointer
        {
            public float* ptr;
        };

        List<MaterialPropertyType> m_MaterialPropertyTypes;
        ArchetypeChunkComponentTypeDynamic[] m_MaterialPropertyComponentTypes;
        MaterialPropertyPointer[] m_MaterialPropertyPointers;

        public InstancedRenderMeshBatchGroup(EntityManager entityManager, ComponentSystemBase componentSystem, EntityQuery cullingJobDependencyGroup)
        {
            m_BatchRendererGroup = new BatchRendererGroup(this.OnPerformCulling);
            m_EntityManager = entityManager;
            m_ComponentSystem = componentSystem;
            m_CullingJobDependencyGroup = cullingJobDependencyGroup;
            m_BatchToChunkMap = new NativeMultiHashMap<LocalGroupKey, BatchChunkData>(32, Allocator.Persistent);
            m_LocalIdPool = new NativeArray<int>(kMaxBatchCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_Tags = new NativeArray<FrozenRenderSceneTag>(kMaxBatchCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_ForceLowLOD = new NativeArray<byte>(kMaxBatchCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_InternalToExternalIds = new NativeArray<int>(kMaxBatchCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_ExternalToInternalIds = new NativeArray<int>(kMaxBatchCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_ResetLod = true;
            m_InternalBatchRange = 0;
            m_ExternalBatchCount = 0;

            m_RemoveBatchMarker = new ProfilerMarker("BatchRendererGroup.Remove");

#if UNITY_EDITOR
            m_CullingStats = (CullingStats*)UnsafeUtility.Malloc(JobsUtility.MaxJobThreadCount * sizeof(CullingStats), 64, Allocator.Persistent);
#endif
            m_MaterialPropertyBlocks = new List<MaterialPropertyBlock>();

            // Collect all components with [MaterialProperty] attribute
            m_MaterialPropertyTypes = new List<MaterialPropertyType>();
            foreach (var typeInfo in TypeManager.AllTypes)
            {
                var type = typeInfo.Type;
                if (typeof(IComponentData).IsAssignableFrom(type))
                {
                    var attributes = type.GetCustomAttributes(typeof(MaterialPropertyAttribute), false);
                    if (attributes.Length > 0)
                    {
                        var format = ((MaterialPropertyAttribute)attributes[0]).Format;
                        int numFormatComponents = 1;
                        switch (format)
                        {
                            case MaterialPropertyFormat.Float: numFormatComponents = 1; break;
                            case MaterialPropertyFormat.Float4: numFormatComponents = 4; break;
                            case MaterialPropertyFormat.Float4x4: numFormatComponents = 16; break;
                        }

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                        if (UnsafeUtility.SizeOf(type) != numFormatComponents * sizeof(float))
                        {
                            throw new InvalidOperationException($"Material property component {type} (size = {UnsafeUtility.SizeOf(type)}) cannot be reinterpreted as {numFormatComponents} floats (size = {numFormatComponents * sizeof(float)}). Sizes must match.");
                        }
#endif

                        m_MaterialPropertyTypes.Add(new MaterialPropertyType { typeIndex = TypeManager.GetTypeIndex(type), nameId = Shader.PropertyToID(((MaterialPropertyAttribute)attributes[0]).Name + "_Array"), format = format, numFormatComponents = numFormatComponents });
                    }
                }
            }

            m_MaterialPropertyComponentTypes = new ArchetypeChunkComponentTypeDynamic[m_MaterialPropertyTypes.Count];
            m_MaterialPropertyPointers = new MaterialPropertyPointer[m_MaterialPropertyTypes.Count];

            ResetLocalIdPool();
        }

        private void ResetLocalIdPool()
        {
            m_LocalIdCapacity = kMaxBatchCount;
            for (int i = 0; i < kMaxBatchCount; ++i)
            {
                m_LocalIdPool[i] = kMaxBatchCount - i - 1;
            }
        }

        public void Dispose()
        {
#if UNITY_EDITOR
            UnsafeUtility.Free(m_CullingStats, Allocator.Persistent);

            m_CullingStats = null;
#endif
            m_LocalIdPool.Dispose();
            m_ExternalToInternalIds.Dispose();
            m_InternalToExternalIds.Dispose();
            m_BatchRendererGroup.Dispose();
            m_BatchToChunkMap.Dispose();
            m_Tags.Dispose();
            m_ForceLowLOD.Dispose();
            m_ResetLod = true;
            m_InternalBatchRange = 0;
            m_ExternalBatchCount = 0;
            m_MaterialPropertyBlocks.Clear();
        }

        public void Clear()
        {
            m_BatchRendererGroup.Dispose();
            m_BatchRendererGroup = new BatchRendererGroup(this.OnPerformCulling);
            m_PrevLODParams = new LODGroupExtensions.LODParams();
            m_PrevCameraPos = default(float3);
            m_PrevLodDistanceScale = 0.0f;
            m_ResetLod = true;
            m_InternalBatchRange = 0;
            m_ExternalBatchCount = 0;

            m_BatchToChunkMap.Clear();
            m_MaterialPropertyBlocks.Clear();

            ResetLocalIdPool();
        }

        public int AllocLocalId()
        {
            Assert.IsTrue(m_LocalIdCapacity > 0);
            int result = m_LocalIdPool[m_LocalIdCapacity - 1];
            --m_LocalIdCapacity;
            return result;
        }

        public void FreeLocalId(int id)
        {
            Assert.IsTrue(m_LocalIdCapacity < kMaxBatchCount);
            int result = m_LocalIdPool[m_LocalIdCapacity] = id;
            ++m_LocalIdCapacity;
        }

        public void ResetLod()
        {
            m_PrevLODParams = new LODGroupExtensions.LODParams();
            m_ResetLod = true;
        }

        public unsafe JobHandle OnPerformCulling(BatchRendererGroup rendererGroup, BatchCullingContext cullingContext)
        {
#if false
            // Reset all visible counts to 0 - to not crash Unity when there is bugs in this code during dev.
            for (int i = 0; i < cullingContext.batchVisibility.Length; ++i)
            {
                var v = cullingContext.batchVisibility[i];
                v.visibleCount = 0;
                cullingContext.batchVisibility[i] = v;
            }
#endif

            if (LastUpdatedOrderVersion != m_EntityManager.GetComponentOrderVersion<RenderMesh>())
            {
                // Debug.LogError("The chunk layout of RenderMesh components has changed between updating and culling. This is not allowed, rendering is disabled.");
                return default(JobHandle);
            }

            var batchCount = cullingContext.batchVisibility.Length;
            if (batchCount == 0)
                return new JobHandle(); ;

            var lodParams = LODGroupExtensions.CalculateLODParams(cullingContext.lodParameters);

            Profiler.BeginSample("OnPerformCulling");

            int cullingPlaneCount = cullingContext.cullingPlanes.Length;
            int packetCount = (cullingPlaneCount + 3) >> 2;
            var planes = FrustumPlanes.BuildSOAPlanePackets(cullingContext.cullingPlanes, Allocator.TempJob);

            bool singleThreaded = false;

            JobHandle cullingDependency;
            var resetLod = m_ResetLod || (!lodParams.Equals(m_PrevLODParams));
            if (resetLod)
            {
                // Depend on all component ata we access + previous jobs since we are writing to a single
                // m_ChunkInstanceLodEnableds array.
                var lodJobDependency = JobHandle.CombineDependencies(m_CullingJobDependency, m_CullingJobDependencyGroup.GetDependency());

                float cameraMoveDistance = math.length(m_PrevCameraPos - lodParams.cameraPos);
                var lodDistanceScaleChanged = lodParams.distanceScale != m_PrevLodDistanceScale;

#if UNITY_EDITOR
                // Record this separately in the editor for stats display
                m_CamMoveDistance = cameraMoveDistance;
#endif

                var selectLodEnabledJob = new SelectLodEnabled
                {
                    ForceLowLOD = m_ForceLowLOD,
                    LODParams = lodParams,
                    RootLodRequirements = m_ComponentSystem.GetArchetypeChunkComponentType<RootLodRequirement>(true),
                    InstanceLodRequirements = m_ComponentSystem.GetArchetypeChunkComponentType<LodRequirement>(true),
                    CameraMoveDistanceFixed16 = Fixed16CamDistance.FromFloatCeil(cameraMoveDistance * lodParams.distanceScale),
                    DistanceScale = lodParams.distanceScale,
                    DistanceScaleChanged = lodDistanceScaleChanged,
#if UNITY_EDITOR
                    Stats = m_CullingStats,
#endif
                };

                cullingDependency = m_LODDependency = selectLodEnabledJob.Schedule(m_BatchToChunkMap, singleThreaded ? 150000 : m_BatchToChunkMap.Capacity / 64, lodJobDependency);

                m_PrevLODParams = lodParams;
                m_PrevLodDistanceScale = lodParams.distanceScale;
                m_PrevCameraPos = lodParams.cameraPos;
                m_ResetLod = false;
#if UNITY_EDITOR
                UnsafeUtility.MemClear(m_CullingStats, sizeof(CullingStats) * JobsUtility.MaxJobThreadCount);
#endif
            }
            else
            {
                // Depend on all component ata we access + previous m_LODDependency job
                cullingDependency = JobHandle.CombineDependencies(m_LODDependency, m_CullingJobDependencyGroup.GetDependency());
            }

            var batchCullingStates = new NativeArray<BatchCullingState>(m_InternalBatchRange, Allocator.TempJob, NativeArrayOptions.ClearMemory);

            var simpleCullingJob = new SimpleCullingJob
            {
                Planes = planes,
                BatchCullingStates = batchCullingStates,
                BoundsComponent = m_ComponentSystem.GetArchetypeChunkComponentType<WorldRenderBounds>(true),
                IndexList = cullingContext.visibleIndices,
                Batches = cullingContext.batchVisibility,
                InternalToExternalRemappingTable = m_InternalToExternalIds,
#if UNITY_EDITOR
                Stats = m_CullingStats,
#endif
            };

            var simpleCullingJobHandle = simpleCullingJob.Schedule(m_BatchToChunkMap, singleThreaded ? 150000 : 1024, cullingDependency);

            DidScheduleCullingJob(simpleCullingJobHandle);

            Profiler.EndSample();
            return simpleCullingJobHandle;
        }

        public void BeginBatchGroup()
        {

        }

        public unsafe void AddBatch(FrozenRenderSceneTag tag, int rendererSharedComponentIndex, int batchInstanceCount, NativeArray<ArchetypeChunk> chunks, NativeArray<int> sortedChunkIndices, int startSortedIndex, int chunkCount, bool flippedWinding, EditorRenderData data)
        {
            var bigBounds = new Bounds(new Vector3(0, 0, 0), new Vector3(16738.0f, 16738.0f, 16738.0f));

            var rendererSharedComponent = m_EntityManager.GetSharedComponentData<RenderMesh>(rendererSharedComponentIndex);
            var mesh = rendererSharedComponent.mesh;
            var material = rendererSharedComponent.material;
            var castShadows = rendererSharedComponent.castShadows;
            var receiveShadows = rendererSharedComponent.receiveShadows;
            var subMeshIndex = rendererSharedComponent.subMesh;
            var layer = rendererSharedComponent.layer;

            if (mesh == null || material == null)
            {
                return;
            }

            Profiler.BeginSample("AddBatch");
            int externalBatchIndex = m_BatchRendererGroup.AddBatch(mesh, subMeshIndex, material, layer, castShadows, receiveShadows, flippedWinding, bigBounds, batchInstanceCount, null, data.PickableObject, data.SceneCullingMask);

            if (externalBatchIndex > m_MaterialPropertyBlocks.Count - 1)
                m_MaterialPropertyBlocks.Add(new MaterialPropertyBlock());

            var propertyBlock = m_MaterialPropertyBlocks[externalBatchIndex];
            m_BatchRendererGroup.SetInstancingData(externalBatchIndex, batchInstanceCount, propertyBlock);

            var matrices = (float4x4*)m_BatchRendererGroup.GetBatchMatrices(externalBatchIndex).GetUnsafePtr();
            Profiler.EndSample();

            int internalBatchIndex = AllocLocalId();
            //Debug.Log($"Adding internal index {internalBatchIndex} for external index {externalBatchIndex}; pre count {m_ExternalBatchCount}");

            m_ExternalToInternalIds[externalBatchIndex] = internalBatchIndex;
            m_InternalToExternalIds[internalBatchIndex] = externalBatchIndex;

            var boundsType = m_ComponentSystem.GetArchetypeChunkComponentType<ChunkWorldRenderBounds>(true);
            var localToWorldType = m_ComponentSystem.GetArchetypeChunkComponentType<LocalToWorld>(true);
            var rootLodRequirements = m_ComponentSystem.GetArchetypeChunkComponentType<RootLodRequirement>(true);
            var instanceLodRequirements = m_ComponentSystem.GetArchetypeChunkComponentType<LodRequirement>(true);
            var perInstanceCullingTag = m_ComponentSystem.GetArchetypeChunkComponentType<PerInstanceCullingTag>(true);

            int runningOffset = 0;

            for (int i = 0; i < m_MaterialPropertyPointers.Length; i++)
            {
                // Get archetype chunk components based on dynamic reflection type id
                m_MaterialPropertyComponentTypes[i] = m_ComponentSystem.GetArchetypeChunkComponentTypeDynamic(ComponentType.ReadOnly(m_MaterialPropertyTypes[i].typeIndex));
                m_MaterialPropertyPointers[i] = new MaterialPropertyPointer { ptr = null };
            }

            var previousArchetype = new EntityArchetype();
            int numActiveArchetypeMaterialProperties = 0;
            var archetypeActiveMaterialProperties = new NativeArray<int>(kMaxArchetypeProperties, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            for (int i = 0; i < chunkCount; ++i)
            {
                var chunk = chunks[sortedChunkIndices[startSortedIndex + i]];
                var bounds = chunk.GetChunkComponentData(boundsType);

                var localKey = new LocalGroupKey { Value = internalBatchIndex };
                var hasLodData = chunk.Has(rootLodRequirements) && chunk.Has(instanceLodRequirements);
                var hasPerInstanceCulling = !hasLodData || chunk.Has(perInstanceCullingTag);

                Assert.IsTrue(chunk.Count <= 128);

                m_BatchToChunkMap.Add(localKey, new BatchChunkData
                {
                    Chunk = chunk,
                    Flags = (byte)((hasLodData ? BatchChunkData.kFlagHasLodData : 0) | (hasPerInstanceCulling ? BatchChunkData.kFlagInstanceCulling : 0)),
                    ChunkBounds = bounds,
                    ChunkInstanceCount = (short)chunk.Count,
                    BatchOffset = (short)runningOffset,
                    InstanceLodEnableds = default
                });

                var matrixSizeOf = UnsafeUtility.SizeOf<float4x4>();
                var localToWorld = chunk.GetNativeArray(localToWorldType);
                float4x4* srcMatrices = (float4x4*)localToWorld.GetUnsafeReadOnlyPtr();

                UnsafeUtility.MemCpy(matrices, srcMatrices, matrixSizeOf * chunk.Count);

                matrices += chunk.Count;

                // Go though all [MaterialProperty] component types in the chunk, and collect them to an array. Use same components until archetype changes.
                if (previousArchetype != chunk.Archetype)
                {
                    previousArchetype = chunk.Archetype;
                    numActiveArchetypeMaterialProperties = 0;

                    for (int j = 0; j < m_MaterialPropertyPointers.Length; j++)
                    {
                        var exists = chunk.Has(m_MaterialPropertyComponentTypes[j]);
                        if (exists)
                        {
                            // Lazy allocate C++ arrays only for used [MaterialProperty] components
                            if (m_MaterialPropertyPointers[j].ptr == null)
                            {
                                switch (m_MaterialPropertyTypes[j].format)
                                {
                                    case MaterialPropertyFormat.Float:
                                        var arr1 = m_BatchRendererGroup.GetBatchScalarArray(externalBatchIndex, m_MaterialPropertyTypes[j].nameId);
                                        m_MaterialPropertyPointers[j] = new MaterialPropertyPointer { ptr = (float*)arr1.GetUnsafeReadOnlyPtr() };
                                        break;

                                    case MaterialPropertyFormat.Float4:
                                        var arr4 = m_BatchRendererGroup.GetBatchVectorArray(externalBatchIndex, m_MaterialPropertyTypes[j].nameId);
                                        m_MaterialPropertyPointers[j] = new MaterialPropertyPointer { ptr = (float*)arr4.GetUnsafeReadOnlyPtr() };
                                        break;

                                    case MaterialPropertyFormat.Float4x4:
                                        var arr4x4 = m_BatchRendererGroup.GetBatchMatrixArray(externalBatchIndex, m_MaterialPropertyTypes[j].nameId);
                                        m_MaterialPropertyPointers[j] = new MaterialPropertyPointer { ptr = (float*)arr4x4.GetUnsafeReadOnlyPtr() };
                                        break;
                                }
                            }

                            archetypeActiveMaterialProperties[numActiveArchetypeMaterialProperties++] = j;
                        }
                    }
                }

                // Memcpy all material property instance data in [MaterialProperty] types to C++ side arrays
                for (int j = 0; j < numActiveArchetypeMaterialProperties; j++)
                {
                    var componentIndex = archetypeActiveMaterialProperties[j];
                    var componentSize = UnsafeUtility.SizeOf<float>() * m_MaterialPropertyTypes[componentIndex].numFormatComponents;
                    var chunkData = chunk.GetDynamicComponentDataArrayReinterpret<float>(m_MaterialPropertyComponentTypes[componentIndex], componentSize);
                    Debug.Assert(chunkData.Length > 0);

                    float* srcData = (float*)chunkData.GetUnsafeReadOnlyPtr();
                    float* dstData = m_MaterialPropertyPointers[componentIndex].ptr + runningOffset * m_MaterialPropertyTypes[componentIndex].numFormatComponents;
                    int copySize = chunk.Count * componentSize;

                    UnsafeUtility.MemCpy(dstData, srcData, copySize);
                }

                runningOffset += chunk.Count;
            }

            archetypeActiveMaterialProperties.Dispose();

            m_Tags[internalBatchIndex] = tag;
            m_ForceLowLOD[internalBatchIndex] = (byte)((tag.SectionIndex == 0 && tag.HasStreamedLOD != 0) ? 1 : 0);

            m_InternalBatchRange = math.max(m_InternalBatchRange, internalBatchIndex + 1);
            m_ExternalBatchCount = externalBatchIndex + 1;

            SanityCheck();
        }

        private void SanityCheck()
        {
#if false
            //Debug.Log($"SanityCheck ir {m_InternalBatchRange} ec {m_ExternalBatchCount}");
            Assert.IsTrue(m_InternalBatchRange >= m_ExternalBatchCount);

            var populated = 0;

            var lookup = new Dictionary<int, bool>();

            for (int i = 0; i < m_InternalBatchRange; ++i)
            {
                var internalId = i;
                var externalId = m_InternalToExternalIds[i];
                if (externalId == -1)
                    continue;
                if (externalId >= m_ExternalBatchCount)
                {
                    Debug.Log($"Invalid external id {externalId} for internal id {i} (max {m_ExternalBatchCount})");
                }
                else
                {
                    if (lookup.ContainsKey(externalId))
                    {
                        Debug.Log($"Duplicate mapping e={externalId} at internal id {i}");
                    }
                    else
                    {
                        lookup.Add(externalId, true);
                    }
                }
            }

            if (lookup.Count != m_ExternalBatchCount)
            {
                Debug.Log($"Unreachable external batches: have {lookup.Count} but need {m_ExternalBatchCount}");
            }

            lookup.Clear();

            for (int i = 0; i < m_ExternalBatchCount; ++i)
            {
                var externalId = i;
                var internalId = m_ExternalToInternalIds[i];
                if (internalId < 0 || internalId >= m_InternalBatchRange)
                {
                    Debug.Log($"Invalid internal id {internalId} for external id {externalId}");
                }
                else
                {
                    if (lookup.ContainsKey(internalId))
                    {
                        Debug.Log($"Duplicate mapping e={externalId} to internal id {internalId}");
                    }
                    else
                    {
                        lookup.Add(internalId, true);
                    }
                }

                var ext2 = m_InternalToExternalIds[internalId];
                if (ext2 != externalId)
                {
                    Debug.Log($"Invalid round trip for internal id {internalId} for external id {externalId}; got {ext2}");
                }
            }

            if (lookup.Count != m_ExternalBatchCount)
            {
                Debug.Log($"Bad count of internal batches: have {lookup.Count} but need {m_ExternalBatchCount}");
            }
#endif
        }

        public void EndBatchGroup(FrozenRenderSceneTag tag, NativeArray<ArchetypeChunk> chunks, NativeArray<int> sortedChunkIndices)
        {
            // Disable force low lod  based on loading a streaming zone
            if (tag.SectionIndex > 0 && tag.HasStreamedLOD != 0)
            {
                for (int i = 0; i < m_InternalBatchRange; i++)
                {
                    if (m_Tags[i].SceneGUID.Equals(tag.SceneGUID))
                    {
                        m_ForceLowLOD[i] = 0;
                    }
                }
            }
        }

        public void RemoveTag(FrozenRenderSceneTag tag)
        {
            // Enable force low lod based on the high lod being streamed out
            if (tag.SectionIndex > 0 && tag.HasStreamedLOD != 0)
            {
                for (int i = 0; i < m_InternalBatchRange; i++)
                {
                    if (m_Tags[i].SceneGUID.Equals(tag.SceneGUID))
                    {
                        m_ForceLowLOD[i] = 1;
                    }
                }
            }

            Profiler.BeginSample("RemoveTag");
            // Remove any tag that need to go
            for (int i = m_InternalBatchRange - 1; i >= 0; i--)
            {
                var shouldRemove = m_Tags[i].Equals(tag);
                if (!shouldRemove)
                    continue;

                var externalBatchIndex = m_InternalToExternalIds[i];
                if (externalBatchIndex == -1)
                    continue;

                //Debug.Log($"Removing internal index {i} for external index {externalBatchIndex}; pre batch count = {m_ExternalBatchCount}");

                m_RemoveBatchMarker.Begin();
                m_BatchRendererGroup.RemoveBatch(externalBatchIndex);
                m_RemoveBatchMarker.End();

                // I->E: [ x: 0, y: 1, z: 2 ]  -> [ x: 0, y: ?, z: 2 ]
                // E->I: [ 0: x, 1: y, 2: z ]  -> [ 0: x, 1: z ]
                // B:    [ A B C ]             -> [ A C ]


                // Update remapping for external block. The render group will swap with the end, so replicate that behavior.
                var swappedInternalId = m_ExternalToInternalIds[m_ExternalBatchCount - 1];

                m_ExternalToInternalIds[externalBatchIndex] = swappedInternalId;
                m_InternalToExternalIds[swappedInternalId] = externalBatchIndex;

                // Return local id to pool
                FreeLocalId(i);

                // Invalidate id remapping table for this internal id
                m_InternalToExternalIds[i] = -1;

                m_Tags[i] = default(FrozenRenderSceneTag);

                var localKey = new LocalGroupKey { Value = i };
                m_BatchToChunkMap.Remove(localKey);

                m_ExternalBatchCount--;
            }

            Profiler.EndSample();

            SanityCheck();
        }

        public void CompleteJobs()
        {
            m_CullingJobDependency.Complete();
            m_CullingJobDependencyGroup.CompleteDependency();
        }


        void DidScheduleCullingJob(JobHandle job)
        {
            m_CullingJobDependency = JobHandle.CombineDependencies(job, m_CullingJobDependency);
            m_CullingJobDependencyGroup.AddDependency(job);
        }

    }
#endif

}
