using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class TeamMovementSystem : SystemBase
{

    // TODO : This is the same as the omniworker. Merge if no change is made
    protected override void OnUpdate()
    {
        var deltatime = Time.DeltaTime;

        // var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var sys = this.World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var parallelEcb = sys.CreateCommandBuffer().AsParallelWriter();

        Entities
            .WithAny<BucketFetcher, EmptyBucketWorker, FullBucketWorker>()
            .ForEach((int entityInQueryIndex, ref Translation currentWorkerPosition, in TargetPosition targetPosition, in HeldBucket heldBucket) =>
            {
                var direction = math.normalize(
                    new float2(
                        targetPosition.Value.x - currentWorkerPosition.Value.x, 
                        targetPosition.Value.z - currentWorkerPosition.Value.z));

                var distance =
                    math.sqrt(
                        math.pow(targetPosition.Value.z - currentWorkerPosition.Value.z, 2) +
                        math.pow(targetPosition.Value.x - currentWorkerPosition.Value.x, 2));

                // We only move is the target position is not equal to the current position
                if (!float.IsNaN(direction.x) && !float.IsNaN(direction.y))
                {
                    // if close to the target position, we don't want to overshoot the target, so set the position to the target position.
                    var speed = heldBucket.Bucket == Entity.Null? Const.SpeedFast : (GetComponent<Bucket>(heldBucket.Bucket).HasWater ? Const.SpeedSlow : Const.SpeedFast);
                    if (distance < speed * (float) deltatime)
                    {
                        currentWorkerPosition.Value.x = targetPosition.Value.x;
                        currentWorkerPosition.Value.z = targetPosition.Value.z;
                    }
                    else
                    {
                        currentWorkerPosition.Value.x = currentWorkerPosition.Value.x + (float)deltatime * speed * direction.x;
                        currentWorkerPosition.Value.z = currentWorkerPosition.Value.z + (float)deltatime * speed * direction.y;
                    }

                    // move bucket along with the worker
                    if (heldBucket.Bucket != Entity.Null)
                        parallelEcb.SetComponent<Translation>(entityInQueryIndex, heldBucket.Bucket, new Translation{Value = new float3(currentWorkerPosition.Value.x, 2.25f, currentWorkerPosition.Value.z)});
                }
            }).ScheduleParallel();

        sys.AddJobHandleForProducer(this.Dependency);
        // this.Dependency.Complete();
        // ecb.Playback(EntityManager);
        // ecb.Dispose();
    }
}