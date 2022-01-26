using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class SetLakeAsTargetSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    private EntityQuery LakeQuery;

    protected override void OnCreate()
    {
        LakeQuery = GetEntityQuery(ComponentType.ReadOnly<Lake>(), ComponentType.ReadOnly<Translation>());
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var config = GetSingleton<GameConstants>();
        var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        //var chunks = LakeQuery.CreateArchetypeChunkArray(Allocator.TempJob);

        var lakeTranslations = LakeQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        // TODO: if there are no flames don't do anything
        Entities
            .WithAll<HoldsEmptyBucket, BucketFetcher>()
            .WithNone<TargetDestination>()
            .WithReadOnly(lakeTranslations)
            .WithDisposeOnCompletion(lakeTranslations)
            .ForEach((Entity e, int entityInQueryIndex, in Translation translation) =>
            {
                // HACK: We assume that a flame exists here...
                var closest = new float2(10000000, 100000); // This is bad HACK
                var bestDistance = float.MaxValue;
                // HACK: We are mixing types, this is awful.

                for (int i = 0; i < lakeTranslations.Length; i++)
                {
                    var dist = math.distance(lakeTranslations[i].Value, translation.Value);

                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        closest = lakeTranslations[i].Value.xz;
                    }
                }

                ecb.AddComponent(entityInQueryIndex, e, new TargetDestination { Value = closest });

            }).ScheduleParallel();


        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
