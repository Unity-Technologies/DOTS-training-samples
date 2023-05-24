using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateBefore(typeof(TransformSystemGroup)), UpdateAfter(typeof(TeamUpdateSystem))]
public partial struct RunnerUpdateSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (runnerState, transform, entity) in SystemAPI.Query<
                         RefRW<RunnerState>, 
                         RefRW<LocalTransform>>()
                     .WithEntityAccess())
        {
            switch (runnerState.ValueRO.State)
            {
                case RunnerStates.MovingBucket:
                {
                    // Return the bucket to the water target!
                    var target = runnerState.ValueRO.WaterPosition;
                    if (!Movement.MoveToPosition(ref target, ref transform.ValueRW, SystemAPI.Time.DeltaTime))
                    {
                        // Haven't reached it yet? Keep going!
                        continue;
                    }

                    // We've reached it: fall through to the idle case so we'll pick a new bucket.
                    goto case RunnerStates.Idle;
                }
                case RunnerStates.Idle:
                {
                    // Pick a new bucket!
                    var random = Random.CreateFromIndex((uint)SystemAPI.Time.ElapsedTime);
                    runnerState.ValueRW.TargetBucketPosition = GetRandomBucketPosition(ref state, random);
                    runnerState.ValueRW.State = RunnerStates.FetchingBucket;
                    break;
                }
                case RunnerStates.FetchingBucket:
                {
                    // Move towards the bucket.

                    var target = runnerState.ValueRO.TargetBucketPosition;
                    var oldPosition = transform.ValueRO.Position;
                    if (!Movement.MoveToPosition(ref target, ref transform.ValueRW, SystemAPI.Time.DeltaTime))
                    {
                        // Haven't reached it yet? Keep going!
                        var newPosition = transform.ValueRO.Position;
                        Debug.Log($"Runner fetching moved from {oldPosition.x}, {oldPosition.z} to {newPosition.x}, {newPosition.z}");
                        continue;
                    }

                    // Reached the bucket? Time to move it!
                    runnerState.ValueRW.State = RunnerStates.MovingBucket;
                    break;
                }
            }
        }
    }
    
    [BurstCompile]
    float2 GetRandomBucketPosition(ref SystemState state, Random random)
    {
        var query = SystemAPI.QueryBuilder().WithAll<BucketData, LocalToWorld>().Build();
        var transforms = query.ToComponentDataArray<LocalToWorld>(Allocator.Temp);
        var randomIndex = random.NextInt(0, transforms.Length);
        return transforms[randomIndex].Position.xz;
    }
}