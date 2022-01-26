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

    private static float3 FindClosestTranslation(Translation origin, NativeArray<Translation>.ReadOnly translations)
    {
        int closestIndex = 0;
        var bestDistanceSquared = float.MaxValue;

        for (int i = 0; i < translations.Length; i++)
        {
            var distsq = math.distancesq(translations[i].Value, origin.Value);

            if (distsq < bestDistanceSquared)
            {
                bestDistanceSquared = distsq;
                closestIndex = i;
            }
        }

        return translations[closestIndex].Value;
    }

    protected override void OnUpdate()
    {
        var lakeTranslations = LakeQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var ecb = CommandBufferSystem.CreateCommandBuffer();

        Entities
            .WithReadOnly(lakeTranslations)
            .WithDisposeOnCompletion(lakeTranslations)
            .ForEach((ref LineLakePosition lineLakePosition, in Translation translation) =>
            {
                lineLakePosition.Value = FindClosestTranslation(translation, lakeTranslations.AsReadOnly());
            })
            .ScheduleParallel();

        var fireTranslations = FireQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        Entities
            .WithReadOnly(fireTranslations)
            .WithDisposeOnCompletion(fireTranslations)
            .ForEach((ref LineFirePosition lineFirePosition, in Translation translation) =>
            {
                lineFirePosition.Value = FindClosestTranslation(translation, fireTranslations.AsReadOnly());
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
                    float t = (float)(x + 1) / (gameConstants.WorkersPerLine + 1);
                    var target = (math.lerp(lineLakePosition.Value, lineFirePosition.Value, t) + math.sin(t * math.PI) * offset + offset);

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
                    float t = (float)(x + 1) / (gameConstants.WorkersPerLine + 2);
                    var target = (math.lerp(lineLakePosition.Value, lineFirePosition.Value, t) - math.sin(t * math.PI) * offset + offset);

                    // DON'T REMOVE THIS LINE
                    TeamWorkers workerEntity = workersBuffer[x + gameConstants.WorkersPerLine];

                    if (HasComponent<TargetDestination>(workerEntity))
                        SetComponent<TargetDestination>(workerEntity, target.xz);
                    else
                        ecb.AddComponent<TargetDestination>(workerEntity, target.xz);
                }
            })
            .Schedule();

        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
