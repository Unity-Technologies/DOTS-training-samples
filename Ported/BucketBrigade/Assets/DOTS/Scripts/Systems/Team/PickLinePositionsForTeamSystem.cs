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
        int closestIndex = 0;
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

    protected override void OnUpdate()
    {
        var lakeTranslations = LakeQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var lakeEntities = LakeQuery.ToEntityArray(Allocator.TempJob);
        var ecb = CommandBufferSystem.CreateCommandBuffer();

        Entities
            .WithReadOnly(lakeTranslations)
            .WithDisposeOnCompletion(lakeTranslations)
            .WithReadOnly(lakeEntities)
            .WithDisposeOnCompletion(lakeEntities)
            .ForEach((ref LineLakePosition lineLakePosition, in Translation translation) =>
            {
                var index = FindClosestTranslationIndex(translation.Value, lakeTranslations.AsReadOnly());

                lineLakePosition.Value = lakeTranslations[index].Value;
                lineLakePosition.Lake = lakeEntities[index];
            })
            .ScheduleParallel();

        var fireTranslations = FireQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        Entities
            .WithReadOnly(fireTranslations)
            .WithDisposeOnCompletion(fireTranslations)
            .ForEach((ref LineFirePosition lineFirePosition, in Translation translation) =>
            {
                var index = FindClosestTranslationIndex(translation.Value, fireTranslations.AsReadOnly());

                lineFirePosition.Value = fireTranslations[index].Value;
            })
            .ScheduleParallel();

        Entities
            .ForEach((ref Translation translation, in LineLakePosition lineLakePosition, in LineFirePosition lineFirePosition) =>
            {
                translation.Value = math.lerp(lineLakePosition.Value, lineFirePosition.Value, 0.5f);
            })
            .ScheduleParallel();

        var gameConstants = GetSingleton<GameConstants>();

        Entities
            .ForEach((Entity e, in LineLakePosition lineLakePosition, in LineFirePosition lineFirePosition) =>
            {
                DynamicBuffer<TeamWorkers> workersBuffer = GetBuffer<TeamWorkers>(e);

                float3 forward = lineFirePosition.Value - lineLakePosition.Value;
                float3 offset = math.cross(forward, new float3(0, 1, 0)) * 0.1f;

                // Forward Line
                for (int x = 0; x < gameConstants.WorkersPerLine; x++)
                {
                    float t = (float)x / (gameConstants.WorkersPerLine + 1);
                    var target = (math.lerp(lineLakePosition.Value, lineFirePosition.Value, t) + math.sin(t * math.PI) * offset);

                    // DON'T REMOVE THIS LINE
                    TeamWorkers workerEntity = workersBuffer[x];

                    if (HasComponent<TargetDestination>(workerEntity))
                        SetComponent<TargetDestination>(workerEntity, target.xz);
                    else
                        ecb.AddComponent<TargetDestination>(workerEntity, target.xz);
                }

                // Backward Line
                for (int x = 0; x < gameConstants.WorkersPerLine; x++)
                {
                    float t = (float)(x + 1) / (gameConstants.WorkersPerLine + 1);
                    var target = (math.lerp(lineLakePosition.Value, lineFirePosition.Value, t) - math.sin(t * math.PI) * offset);

                    // DON'T REMOVE THIS LINE
                    TeamWorkers workerEntity = workersBuffer[x + gameConstants.WorkersPerLine];

                    if (HasComponent<TargetDestination>(workerEntity))
                        SetComponent<TargetDestination>(workerEntity, target.xz);
                    else
                        ecb.AddComponent<TargetDestination>(workerEntity, target.xz);
                }

                // BucketFetcher
                var bucketFetcherEntity = workersBuffer[gameConstants.WorkersPerLine * 2];

                SetComponent(bucketFetcherEntity, new BucketFetcher { Lake = lineLakePosition.Lake, LakePosition = lineLakePosition.Value });

            })
            .Schedule();

        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
