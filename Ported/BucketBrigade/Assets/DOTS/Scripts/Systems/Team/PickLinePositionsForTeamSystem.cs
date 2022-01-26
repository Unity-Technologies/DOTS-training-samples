using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MoveToTargetLocationSystem))]
public partial class PickLinePositionsForTeamSystem : SystemBase
{
    private EntityQuery LakeQuery;
    private EntityQuery FireQuery;

    protected override void OnCreate()
    {
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
            .ForEach((Entity e, ref DynamicBuffer<TeamWorkers> workersBuffer, in LineLakePosition lineLakePosition, in LineFirePosition lineFirePosition) =>
            {
                // Forward Line
                for (int i = 0; i < gameConstants.WorkersPerLine; i++)
                {
                    float t = (float)i / gameConstants.WorkersPerLine;
                    float2 target = math.lerp(lineLakePosition.Value, lineFirePosition.Value, t).xz;

                    TargetPosition targetPosition = new TargetPosition() { Value = target };

                    SetComponent<TargetPosition>(workersBuffer[i].Value, targetPosition);
                }
            })
            .Schedule();
    }
}
