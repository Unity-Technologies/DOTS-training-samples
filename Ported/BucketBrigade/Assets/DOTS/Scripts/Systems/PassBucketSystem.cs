using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(PickLinePositionsForTeamSystem))]
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
        var gameConstants = GetSingleton<GameConstants>();

        Entities.
            WithNone<HoldsBucketBeingFilled>().
            ForEach((Entity e, in HoldingBucket holdingBucket, in Translation translation, in PassTo passTo, in TargetDestination target) => {
                if (target.DistanceToDestinationSq(translation) > gameConstants.FireFighterBucketPassRadius * gameConstants.FireFighterBucketPassRadius)
                    return;


                // Wait for next worker to do something with bucket
                if (HasComponent<HoldingBucket>(passTo.NextWorker))
                    return;

                if (HasComponent<BucketFetcher>(e))
                {
                    if (!HasComponent<PassToTargetAssigned>(e) ||
                        HasComponent<EmptyBucket>(holdingBucket.HeldBucket))
                        return;

                    ecb.RemoveComponent<PassToTargetAssigned>(e);
                }

                var throwingFullBucket = HasComponent<BucketThrower>(e) && !HasComponent<EmptyBucket>(holdingBucket.HeldBucket);

                //ecb.RemoveComponent<PassToTargetAssigned>(e);
                ecb.RemoveComponent<HoldingBucket>(e);
                ecb.AddComponent(passTo.NextWorker, holdingBucket);

                if (throwingFullBucket)
                {
                    // TODO: make dousing
                    ecb.AddComponent<DousingEvent>(e);

                    ecb.SetComponent(holdingBucket.HeldBucket, new Bucket { Volume = 0 });
                    ecb.AddComponent<EmptyBucket>(holdingBucket.HeldBucket);
                }

                // TODO: Douse fire if this dude was supposed to throw

            }).Schedule();

        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
