using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Networking.Transport.Utilities;
using Unity.NetCode;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(GhostUpdateSystemGroup))]
public class OverlayColorGhostUpdateSystem : JobComponentSystem
{
    [BurstCompile]
    struct UpdateInterpolatedJob : IJobChunk
    {
        [ReadOnly] public NativeHashMap<int, GhostEntity> GhostMap;
        [ReadOnly] public ArchetypeChunkBufferType<OverlayColorSnapshotData> ghostSnapshotDataType;
        [ReadOnly] public ArchetypeChunkEntityType ghostEntityType;
        public ArchetypeChunkComponentType<OverlayColorComponent> ghostOverlayColorComponentType;
        public ArchetypeChunkComponentType<Rotation> ghostRotationType;
        public ArchetypeChunkComponentType<Translation> ghostTranslationType;

        public uint targetTick;
        public float targetTickFraction;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var deserializerState = new GhostDeserializerState
            {
                GhostMap = GhostMap
            };
            var ghostEntityArray = chunk.GetNativeArray(ghostEntityType);
            var ghostSnapshotDataArray = chunk.GetBufferAccessor(ghostSnapshotDataType);
            var ghostOverlayColorComponentArray = chunk.GetNativeArray(ghostOverlayColorComponentType);
            var ghostRotationArray = chunk.GetNativeArray(ghostRotationType);
            var ghostTranslationArray = chunk.GetNativeArray(ghostTranslationType);
            for (int entityIndex = 0; entityIndex < ghostEntityArray.Length; ++entityIndex)
            {
                var snapshot = ghostSnapshotDataArray[entityIndex];
                OverlayColorSnapshotData snapshotData;
                snapshot.GetDataAtTick(targetTick, targetTickFraction, out snapshotData);

                var ghostOverlayColorComponent = ghostOverlayColorComponentArray[entityIndex];
                var ghostRotation = ghostRotationArray[entityIndex];
                var ghostTranslation = ghostTranslationArray[entityIndex];
                ghostOverlayColorComponent.Color = snapshotData.GetOverlayColorComponentColor(deserializerState);
                ghostRotation.Value = snapshotData.GetRotationValue(deserializerState);
                ghostTranslation.Value = snapshotData.GetTranslationValue(deserializerState);
                ghostOverlayColorComponentArray[entityIndex] = ghostOverlayColorComponent;
                ghostRotationArray[entityIndex] = ghostRotation;
                ghostTranslationArray[entityIndex] = ghostTranslation;
            }
        }
    }
    [BurstCompile]
    struct UpdatePredictedJob : IJobChunk
    {
        [ReadOnly] public NativeHashMap<int, GhostEntity> GhostMap;
#pragma warning disable 649
        [NativeSetThreadIndex]
        public int ThreadIndex;
#pragma warning restore 649
        [NativeDisableParallelForRestriction] public NativeArray<uint> minPredictedTick;
        [ReadOnly] public ArchetypeChunkBufferType<OverlayColorSnapshotData> ghostSnapshotDataType;
        [ReadOnly] public ArchetypeChunkEntityType ghostEntityType;
        public ArchetypeChunkComponentType<PredictedGhostComponent> predictedGhostComponentType;
        public ArchetypeChunkComponentType<OverlayColorComponent> ghostOverlayColorComponentType;
        public ArchetypeChunkComponentType<Rotation> ghostRotationType;
        public ArchetypeChunkComponentType<Translation> ghostTranslationType;
        public uint targetTick;
        public uint lastPredictedTick;
        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            var deserializerState = new GhostDeserializerState
            {
                GhostMap = GhostMap
            };
            var ghostEntityArray = chunk.GetNativeArray(ghostEntityType);
            var ghostSnapshotDataArray = chunk.GetBufferAccessor(ghostSnapshotDataType);
            var predictedGhostComponentArray = chunk.GetNativeArray(predictedGhostComponentType);
            var ghostOverlayColorComponentArray = chunk.GetNativeArray(ghostOverlayColorComponentType);
            var ghostRotationArray = chunk.GetNativeArray(ghostRotationType);
            var ghostTranslationArray = chunk.GetNativeArray(ghostTranslationType);
            for (int entityIndex = 0; entityIndex < ghostEntityArray.Length; ++entityIndex)
            {
                var snapshot = ghostSnapshotDataArray[entityIndex];
                OverlayColorSnapshotData snapshotData;
                snapshot.GetDataAtTick(targetTick, out snapshotData);

                var predictedData = predictedGhostComponentArray[entityIndex];
                var lastPredictedTickInst = lastPredictedTick;
                if (lastPredictedTickInst == 0 || predictedData.AppliedTick != snapshotData.Tick)
                    lastPredictedTickInst = snapshotData.Tick;
                else if (!SequenceHelpers.IsNewer(lastPredictedTickInst, snapshotData.Tick))
                    lastPredictedTickInst = snapshotData.Tick;
                if (minPredictedTick[ThreadIndex] == 0 || SequenceHelpers.IsNewer(minPredictedTick[ThreadIndex], lastPredictedTickInst))
                    minPredictedTick[ThreadIndex] = lastPredictedTickInst;
                predictedGhostComponentArray[entityIndex] = new PredictedGhostComponent{AppliedTick = snapshotData.Tick, PredictionStartTick = lastPredictedTickInst};
                if (lastPredictedTickInst != snapshotData.Tick)
                    continue;

                var ghostOverlayColorComponent = ghostOverlayColorComponentArray[entityIndex];
                var ghostRotation = ghostRotationArray[entityIndex];
                var ghostTranslation = ghostTranslationArray[entityIndex];
                ghostOverlayColorComponent.Color = snapshotData.GetOverlayColorComponentColor(deserializerState);
                ghostRotation.Value = snapshotData.GetRotationValue(deserializerState);
                ghostTranslation.Value = snapshotData.GetTranslationValue(deserializerState);
                ghostOverlayColorComponentArray[entityIndex] = ghostOverlayColorComponent;
                ghostRotationArray[entityIndex] = ghostRotation;
                ghostTranslationArray[entityIndex] = ghostTranslation;
            }
        }
    }
    private ClientSimulationSystemGroup m_ClientSimulationSystemGroup;
    private GhostPredictionSystemGroup m_GhostPredictionSystemGroup;
    private EntityQuery m_interpolatedQuery;
    private EntityQuery m_predictedQuery;
    private NativeHashMap<int, GhostEntity> m_ghostEntityMap;
    private GhostUpdateSystemGroup m_GhostUpdateSystemGroup;
    private uint m_LastPredictedTick;
    protected override void OnCreate()
    {
        m_GhostUpdateSystemGroup = World.GetOrCreateSystem<GhostUpdateSystemGroup>();
        m_ghostEntityMap = m_GhostUpdateSystemGroup.GhostEntityMap;
        m_ClientSimulationSystemGroup = World.GetOrCreateSystem<ClientSimulationSystemGroup>();
        m_GhostPredictionSystemGroup = World.GetOrCreateSystem<GhostPredictionSystemGroup>();
        m_interpolatedQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new []{
                ComponentType.ReadWrite<OverlayColorSnapshotData>(),
                ComponentType.ReadOnly<GhostComponent>(),
                ComponentType.ReadWrite<OverlayColorComponent>(),
                ComponentType.ReadWrite<Rotation>(),
                ComponentType.ReadWrite<Translation>(),
            },
            None = new []{ComponentType.ReadWrite<PredictedGhostComponent>()}
        });
        m_predictedQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new []{
                ComponentType.ReadOnly<OverlayColorSnapshotData>(),
                ComponentType.ReadOnly<GhostComponent>(),
                ComponentType.ReadOnly<PredictedGhostComponent>(),
                ComponentType.ReadWrite<OverlayColorComponent>(),
                ComponentType.ReadWrite<Rotation>(),
                ComponentType.ReadWrite<Translation>(),
            }
        });
        RequireForUpdate(GetEntityQuery(ComponentType.ReadWrite<OverlayColorSnapshotData>(),
            ComponentType.ReadOnly<GhostComponent>()));
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!m_predictedQuery.IsEmptyIgnoreFilter)
        {
            var updatePredictedJob = new UpdatePredictedJob
            {
                GhostMap = m_ghostEntityMap,
                minPredictedTick = m_GhostPredictionSystemGroup.OldestPredictedTick,
                ghostSnapshotDataType = GetArchetypeChunkBufferType<OverlayColorSnapshotData>(true),
                ghostEntityType = GetArchetypeChunkEntityType(),
                predictedGhostComponentType = GetArchetypeChunkComponentType<PredictedGhostComponent>(),
                ghostOverlayColorComponentType = GetArchetypeChunkComponentType<OverlayColorComponent>(),
                ghostRotationType = GetArchetypeChunkComponentType<Rotation>(),
                ghostTranslationType = GetArchetypeChunkComponentType<Translation>(),

                targetTick = m_ClientSimulationSystemGroup.ServerTick,
                lastPredictedTick = m_LastPredictedTick
            };
            m_LastPredictedTick = m_ClientSimulationSystemGroup.ServerTick;
            if (m_ClientSimulationSystemGroup.ServerTickFraction < 1)
                m_LastPredictedTick = 0;
            inputDeps = updatePredictedJob.Schedule(m_predictedQuery, JobHandle.CombineDependencies(inputDeps, m_GhostUpdateSystemGroup.LastGhostMapWriter));
            m_GhostPredictionSystemGroup.AddPredictedTickWriter(inputDeps);
        }
        if (!m_interpolatedQuery.IsEmptyIgnoreFilter)
        {
            var updateInterpolatedJob = new UpdateInterpolatedJob
            {
                GhostMap = m_ghostEntityMap,
                ghostSnapshotDataType = GetArchetypeChunkBufferType<OverlayColorSnapshotData>(true),
                ghostEntityType = GetArchetypeChunkEntityType(),
                ghostOverlayColorComponentType = GetArchetypeChunkComponentType<OverlayColorComponent>(),
                ghostRotationType = GetArchetypeChunkComponentType<Rotation>(),
                ghostTranslationType = GetArchetypeChunkComponentType<Translation>(),
                targetTick = m_ClientSimulationSystemGroup.InterpolationTick,
                targetTickFraction = m_ClientSimulationSystemGroup.InterpolationTickFraction
            };
            inputDeps = updateInterpolatedJob.Schedule(m_interpolatedQuery, JobHandle.CombineDependencies(inputDeps, m_GhostUpdateSystemGroup.LastGhostMapWriter));
        }
        return inputDeps;
    }
}
public partial class OverlayColorGhostSpawnSystem : DefaultGhostSpawnSystem<OverlayColorSnapshotData>
{
}
