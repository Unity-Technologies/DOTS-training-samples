using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MoveToTargetLocationSystem))]
public partial class BucketDroppingSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        var gameConstants = GetSingleton<GameConstants>();

        Entities.
            WithAll<BucketDropper>().
            ForEach((Entity e, int entityInQueryIndex, ref HoldingBucket holdingBucket, in Translation t) => 
            {
                ecb.RemoveComponent<HoldingBucket>(entityInQueryIndex, e);
                ecb.RemoveComponent<BeingHeld>(entityInQueryIndex, holdingBucket.HeldBucket);
                var pos = t.Value;
                pos.y = 0;
                ecb.SetComponent(entityInQueryIndex, holdingBucket.HeldBucket, new Translation { Value = pos });
            }).ScheduleParallel();

        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
