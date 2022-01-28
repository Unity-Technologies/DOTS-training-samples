using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(SetLakeAsTargetSystem))]
public partial class BucketFllingSystem : SystemBase
{
    private EntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        CommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var gameConstants = GetSingleton<GameConstants>();
        var deltaTime = Time.DeltaTime;

        var ecb = CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.ForEach((Entity e, int entityInQueryIndex, ref Lake lake, ref DynamicBuffer<BucketFillAction> fillActions) => {

            var drainSpeed = gameConstants.BucketFillRate * deltaTime;

            for(int i = 0; i < fillActions.Length; i++)
            {
                var actionEntry = fillActions[i];
                var toDrain = drainSpeed;

                // TODO: Destroy lake if drained.
                lake.Volume -= toDrain;
                actionEntry.BucketVolume += toDrain;

                ecb.SetComponent(entityInQueryIndex, actionEntry.Bucket, new Bucket { Volume = actionEntry.BucketVolume });

                if (actionEntry.BucketVolume < 1f)
                {
                    fillActions[i] = actionEntry;
                }
                else
                {
                    fillActions.RemoveAt(i);
                    i--;
                    // FireFighter
                    ecb.RemoveComponent<HoldsBucketBeingFilled>(entityInQueryIndex, actionEntry.FireFighter);
                    ecb.AddComponent<PassToTargetAssigned>(entityInQueryIndex, actionEntry.FireFighter);
                    ecb.SetComponent(entityInQueryIndex, actionEntry.FireFighter, (TargetDestination)actionEntry.Position.xz);
                    //ecb.RemoveComponent<HoldingBucket>(entityInQueryIndex, actionEntry.FireFighter);

                    ecb.RemoveComponent<EmptyBucket>(entityInQueryIndex, actionEntry.Bucket);

                    // Bucket
                    //actionEntry.Position.y = 0;
                    //ecb.SetComponent(entityInQueryIndex, actionEntry.Bucket, new Translation { Value = actionEntry.Position });
                    //ecb.RemoveComponent<BeingHeld>(entityInQueryIndex, actionEntry.Bucket);
                }

                if (lake.Volume <= 0)
                {
                    if (!HasComponent<EmptyLake>(e))
                        ecb.AddComponent<EmptyLake>(entityInQueryIndex, e);

                    ecb.RemoveComponent<HoldsBucketBeingFilled>(entityInQueryIndex, actionEntry.FireFighter);

                    fillActions.Clear();
                    return;
                }
            }


        }).ScheduleParallel();

        CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
