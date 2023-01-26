using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MovementSystem))]
[BurstCompile]
partial struct CarriedBucketSystem : ISystem
{
    ComponentLookup<Position> m_PositionLookup;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_PositionLookup = state.GetComponentLookup<Position>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_PositionLookup.Update(ref state);

        var moveBucketJob = new MoveBucketJob
        {
            positionLookup = m_PositionLookup,
        };
        var moveBucketJobHandle = moveBucketJob.Schedule(state.Dependency);

        var updateBucketTransformJob = new UpdateBucketTransformJob();
        state.Dependency = updateBucketTransformJob.Schedule(moveBucketJobHandle);
    }
}



[WithAll(typeof(CarriesBucketTag))]
[BurstCompile]
partial struct MoveBucketJob : IJobEntity
{
    public ComponentLookup<Position> positionLookup;

    void Execute(in Entity worker, in CarriedBucket bucket)
    {
        positionLookup.GetRefRW(bucket.bucket, false).ValueRW.position = positionLookup.GetRefRO(worker).ValueRO.position;
    }
}

[WithAll(typeof(PickedUpTag))]
[BurstCompile]
partial struct UpdateBucketTransformJob : IJobEntity
{
    const float k_BucketHeight = 2;

    void Execute(in Position position, ref TransformAspect transform)
    {
        transform.LocalPosition = new float3(position.position.x, k_BucketHeight, position.position.y);
    }
}
