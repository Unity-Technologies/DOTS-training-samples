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
        var ecb = CommandBufferSystem.CreateCommandBuffer();
        var gameConstants = GetSingleton<GameConstants>();

        Entities.
            WithAll<BucketDropper>().
            ForEach((Entity e, ref HoldingBucket holdingBucket, in Translation t) => 
            {
                ecb.RemoveComponent<HoldingBucket>(e);
                ecb.RemoveComponent<BeingHeld>(holdingBucket.HeldBucket);
                var pos = t.Value;
                pos.y = 0;
                ecb.SetComponent(holdingBucket.HeldBucket, new Translation { Value = pos });
            }).Schedule();

        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
