using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MoveToTargetLocationSystem))]
public partial class PickLinePositionsForTeamSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    private EntityQuery LakeQuery;
    private EntityQuery FireQuery;

    protected override void OnCreate()
    {
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        LakeQuery = GetEntityQuery(ComponentType.ReadOnly<Lake>(), ComponentType.ReadOnly<Translation>());
        FireQuery = GetEntityQuery(ComponentType.ReadOnly<FireRenderer>(), ComponentType.ReadOnly<Translation>());
    }

    private static int FindClosestTranslationIndex(float3 origin, NativeArray<Translation>.ReadOnly translations)
    {
        int closestIndex = -1;
        var bestDistanceSquared = float.MaxValue;

        for (int i = 0; i < translations.Length; i++)
        {
            var distsq = math.distancesq(translations[i].Value, origin);

            if (distsq < bestDistanceSquared)
            {
                bestDistanceSquared = distsq;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    private static float3 GetLinePosition(float t, float3 lineLakePosition, float3 lineFirePosition, float3 offset)
    {
        return math.lerp(lineLakePosition, lineFirePosition, t) + math.sin(t * math.PI) * offset;
    }

    protected override void OnUpdate()
    {
        var lakeTranslations = LakeQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var lakeEntities = LakeQuery.ToEntityArray(Allocator.TempJob);
        var ecb = CommandBufferSystem.CreateCommandBuffer();
        var fireTranslations = FireQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        var canRunTeamSimulation = lakeTranslations.Length > 0 && fireTranslations.Length > 0;

        JobHandle lakePositionJob =
            Entities
            .WithReadOnly(lakeTranslations)
            .WithDisposeOnCompletion(lakeTranslations)
            .WithReadOnly(lakeEntities)
            .WithDisposeOnCompletion(lakeEntities)
            .ForEach((ref LineLakePosition lineLakePosition, in Translation translation) =>
            {
                var index = FindClosestTranslationIndex(translation.Value, lakeTranslations.AsReadOnly());

                if (index == -1)
                    return;

                lineLakePosition.Value = lakeTranslations[index].Value;
                lineLakePosition.Lake = lakeEntities[index];
            })
            .ScheduleParallel(Dependency);


        JobHandle firePositionJob =
            Entities
            .WithReadOnly(fireTranslations)
            .WithDisposeOnCompletion(fireTranslations)
            .ForEach((ref LineFirePosition lineFirePosition, in Translation translation) =>
            {
                var index = FindClosestTranslationIndex(translation.Value, fireTranslations.AsReadOnly());

                if (index == -1)
                    return;

                lineFirePosition.Value = fireTranslations[index].Value;
            })
            .ScheduleParallel(Dependency);

        var gameConstants = GetSingleton<GameConstants>();

        Dependency = JobHandle.CombineDependencies(lakePositionJob, firePositionJob);

        if (canRunTeamSimulation)
        {
            Dependency = Entities
                .ForEach((Entity e, ref Translation translation, in LineLakePosition lineLakePosition, in LineFirePosition lineFirePosition) =>
                {
                // Reposition the team based on picked line positions
                translation.Value = math.lerp(lineLakePosition.Value, lineFirePosition.Value, 0.5f);

                    DynamicBuffer<TeamWorkers> workersBuffer = GetBuffer<TeamWorkers>(e);

                    float3 forward = lineFirePosition.Value - lineLakePosition.Value;
                    float3 offset = math.cross(forward, new float3(0, 1, 0)) * 0.1f;

                // Forward Line
                for (int x = 0; x < gameConstants.WorkersPerLine; x++)
                    {
                        float t = (float)x / gameConstants.WorkersPerLine;
                        float tNext = (float)(x + 1) / gameConstants.WorkersPerLine;

                        var pos = GetLinePosition(t, lineLakePosition.Value, lineFirePosition.Value, offset);
                        var posNext = GetLinePosition(tNext, lineLakePosition.Value, lineFirePosition.Value, offset);


                    // DON'T REMOVE THIS LINE
                    TeamWorkers workerEntity = workersBuffer[x];

                        var isWorkerPassingBucket = HasComponent<HoldingBucket>(workerEntity) && HasComponent<PassTo>(workerEntity);

                        if (isWorkerPassingBucket && !HasComponent<PassToTargetAssigned>(workerEntity))
                            ecb.AddComponent<PassToTargetAssigned>(workerEntity);

                        var targetPosition = (TargetDestination)(isWorkerPassingBucket ? posNext : pos).xz;

                        if (HasComponent<TargetDestination>(workerEntity))
                            SetComponent(workerEntity, targetPosition);
                        else
                            ecb.AddComponent(workerEntity, targetPosition);
                    }

                // Backward Line
                for (int x = 0; x < gameConstants.WorkersPerLine; x++)
                    {
                        float t = (float)x / gameConstants.WorkersPerLine;
                        float tNext = (float)(x + 1) / gameConstants.WorkersPerLine;

                        var pos = GetLinePosition(t, lineFirePosition.Value, lineLakePosition.Value, -offset);
                        var posNext = GetLinePosition(tNext, lineFirePosition.Value, lineLakePosition.Value, -offset);

                    // DON'T REMOVE THIS LINE
                    TeamWorkers workerEntity = workersBuffer[x + gameConstants.WorkersPerLine];

                        var isWorkerPassingBucket = HasComponent<HoldingBucket>(workerEntity) && HasComponent<PassTo>(workerEntity);

                        if (isWorkerPassingBucket && !HasComponent<PassToTargetAssigned>(workerEntity))
                            ecb.AddComponent<PassToTargetAssigned>(workerEntity);

                        var targetPosition = (TargetDestination)(isWorkerPassingBucket ? posNext : pos).xz;

                        if (HasComponent<TargetDestination>(workerEntity))
                            SetComponent(workerEntity, targetPosition);
                        else
                            ecb.AddComponent(workerEntity, targetPosition);
                    }

                // BucketFetcher
                var bucketFetcherEntity = workersBuffer[gameConstants.WorkersPerLine * 2];

                    SetComponent(bucketFetcherEntity, new BucketFetcher { Lake = lineLakePosition.Lake, LakePosition = lineLakePosition.Value });

                })
                .Schedule(Dependency);
        }

        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
