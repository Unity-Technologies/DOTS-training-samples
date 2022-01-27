using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class PassBucketSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = CommandBufferSystem.CreateCommandBuffer();

        Entities.ForEach((Entity e, in HoldingBucket holdingBucket, in Translation translation, in PassTo passTo, in TargetDestination target) => {
            if (math.lengthsq(translation.Value.xz - target.Value) > 0.01f)
                return;

            // Wait for next worker to do something with bucket
            if (HasComponent<HoldingBucket>(passTo.NextWorker))
                return;

            if (HasComponent<HoldsEmptyBucket>(e))
            {
                ecb.RemoveComponent<HoldsEmptyBucket>(e);
                ecb.AddComponent<HoldsEmptyBucket>(passTo.NextWorker);
            }

            if (HasComponent<HoldsFullBucket>(e))
            {
                ecb.RemoveComponent<HoldsFullBucket>(e);
                ecb.AddComponent<HoldsFullBucket>(passTo.NextWorker);
            }

            ecb.RemoveComponent<HoldingBucket>(e);
            ecb.AddComponent(passTo.NextWorker, holdingBucket);

            // TODO: Douse fire if this dude was supposed to throw

        }).Schedule();

        // COPY/PASTE without target destination
        Entities.ForEach((Entity e, in HoldingBucket holdingBucket, in Translation translation, in PassTo passTo) => {
            // Wait for next worker to do something with bucket
            if (HasComponent<HoldingBucket>(passTo.NextWorker))
                return;

            if (HasComponent<HoldsEmptyBucket>(e))
            {
                ecb.RemoveComponent<HoldsEmptyBucket>(e);
                ecb.AddComponent<HoldsEmptyBucket>(passTo.NextWorker);
            }

            if (HasComponent<HoldsFullBucket>(e))
            {
                ecb.RemoveComponent<HoldsFullBucket>(e);
                ecb.AddComponent<HoldsFullBucket>(passTo.NextWorker);
            }

            ecb.RemoveComponent<HoldingBucket>(e);
            ecb.AddComponent(passTo.NextWorker, holdingBucket);

            // TODO: Douse fire if this dude was supposed to throw

        }).Schedule();

        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
