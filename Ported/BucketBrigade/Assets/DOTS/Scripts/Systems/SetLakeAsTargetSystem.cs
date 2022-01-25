using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class SetLakeAsTargetSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var config = GetSingleton<GameConstants>();
        var ecb = CommandBufferSystem.CreateCommandBuffer();

        // TODO: if there are no flames don't do anything
        Entities
            //.WithReadOnly(field)
            .WithAll<HoldsEmptyBucket>()
            .WithNone<TargetDestination>()
            .ForEach((Entity e, in Translation translation) => {
                // HACK: We assume that a flame exists here...
                var closest = new float2(10000000, 100000); // This is bad HACK
                // HACK: We are mixing types, this is awful.

                ecb.AddComponent(e, new TargetDestination { Value = closest });

            }).Schedule();

        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
