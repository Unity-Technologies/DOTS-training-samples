using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using static BucketBrigadeUtility;

public partial class IdleSystem : SystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
    }

    protected override void OnUpdate()
    {
        
        /*
        Filtering the agents in AgentState.idle state.
            In this system, the agents are in-place, and should take the decision of the next action to make.
            For any fetchers and omnis, if they don't own a bucket, find the closest one.  
            Set MyWorkerState to bucketDetection

        For all, if they don't own a bucket, checks if bucketToWant is set.
            they pick up the bucket.
            Otherwise if they own a bucket:
        For fetchers
        if they own an empty bucket
        if they are close to the waterPool
        they switch to AgentState.bucketFill state.
        else
        they define a new TargetRelocation value close to the waterPool.
        else (they own a full bucket)
        if they are close to their transfer target
            they drop the bucket
        else
        they define a new TargetRelocation value close to their transfer target
        For workers
        if they are close to their transfer target, or no transfer target (last worker in "empty bucket" line)
        they drop the bucket
        else
        they define a new TargetRelocation value close to their transfer target
        For captains:
        if the bucket is full
            the bucket is set to BucketState.empty state
        Queue a splash
        else (the bucket is empty)
        if they are close to their transfer target
            they drop the bucket
        else
        they define a new TargetRelocation value close to their transfer target
            */
        
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var helperEntity = GetSingletonEntity<WaterPoolInfo>();
        var waterPoolBuffer = EntityManager.GetBuffer<WaterPoolInfo>(helperEntity);
        
        waterPoolBuffer.Clear();
        
        Entities.WithName("WaterInfoCollect")
            .WithNone<BucketTag>()
            .ForEach((in Entity entity, in Volume volume, in Position position, in Scale scale) =>
            {
                // water pool only counts if it has volume.
                if (volume.Value > 0.01)
                {
                    ecb.AppendToBuffer(helperEntity, new WaterPoolInfo() { WaterPool = entity, Position = position.Value, Radius = scale.Value.x / 2f});
                }
            }).Run();
        
        ecb.Playback(EntityManager);
        ecb.Dispose();
        
        
        waterPoolBuffer = EntityManager.GetBuffer<WaterPoolInfo>(helperEntity);

        var entityManager = EntityManager;

        Entities
            .WithName("Fetcher")
            .WithReadOnly(waterPoolBuffer)
            .WithAll<FetcherTag>()
            .ForEach((ref MyWorkerState state, ref RelocatePosition destination, ref BucketHeld bucketHeld,
                ref BucketToWant bucketWanted, ref MyWaterPool myWaterPool, ref Home home, in Position position,
                in DestinationWorker target) =>
            {
                if (state.Value == WorkerState.Idle)
                {
                    if (bucketHeld.Value == Entity.Null && bucketWanted.Value != Entity.Null)
                    {
                        if (IsEntityVeryClose(entityManager, position.Value, bucketWanted.Value))
                        {
                            PickUpBucket(entityManager, ref bucketHeld, ref bucketWanted);
                        }

                        bucketWanted.Value = Entity.Null;
                        ;
                    }
                    else if (bucketHeld.Value != Entity.Null)
                    {
                        if (IsBucketFull(entityManager, bucketHeld))
                        {
                            var targetPosition = entityManager.GetComponentData<Position>(target.Value);

                            if (IsVeryClose(position.Value, targetPosition.Value))
                            {
                                DropBucket(entityManager, ref bucketHeld);
                            }
                            else
                            {
                                destination.Value = targetPosition.Value;
                            }
                        }
                        else
                        {
                            // goto water source
                            (var waterPool, var waterPosition) = FindClosestWater(position.Value, waterPoolBuffer);

                            if (waterPool != Entity.Null)
                            {
                                if (!IsVeryClose(home.Value, waterPosition))
                                {
                                    home.Value = waterPosition;
                                    // trigger reform
                                }

                                if (IsVeryClose(position.Value, waterPosition))
                                {
                                    myWaterPool.Value = waterPool;
                                    state.Value = WorkerState.FillingBucket;
                                }
                                else
                                {
                                    destination.Value = waterPosition;
                                }
                            }
                        }
                    }
                    else
                    {
                        state.Value = WorkerState.BucketDetection;
                    }
                }
            }).WithoutBurst().Run();
    }
}
