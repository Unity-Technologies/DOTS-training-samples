using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct Worker : IComponentData
{
    public Entity NextWorkerInLine;
}

public struct WorkerMoveTo : IComponentData
{
    public float2 Value;
}

public struct WorkerStartEndPositions : IComponentData
{
    public float2 Start;
    public float2 End;
}

public class WorkerMoveToSystem : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    static float3 MoveTo(float3 pos, float3 dest, float amount)
    {
        float len = math.length(pos - dest);
        if (len < amount)
            return dest;

        float3 moveDir = math.normalize(dest - pos);
        float3 newPos = pos + moveDir * amount;
        return newPos;
    }

    protected override void OnUpdate()
    {
        float speed = 8.0f;
        float deltaTime = Time.DeltaTime;

        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();
        Entities
            .ForEach((int entityInQueryIndex, Entity e, ref Translation pos, in Worker worker, in WorkerMoveTo target) =>
        {
            float3 targetPos = new float3(target.Value.x, pos.Value.y, target.Value.y);
            pos.Value = MoveTo(pos.Value, targetPos, deltaTime * speed);

            if (pos.Value.x == targetPos.x && pos.Value.y == targetPos.y && pos.Value.z == targetPos.z)
            {
                ecb.RemoveComponent<WorkerMoveTo>(entityInQueryIndex, e);
                //note, every worker tries to apply water when it stops moving (not fully correct)
                var extinguishDataEntity = ecb.CreateEntity(entityInQueryIndex);
                ecb.AddComponent(entityInQueryIndex, extinguishDataEntity, new ExtinguishData() {X = (int)pos.Value.x, Y = (int)pos.Value.z});
                ecb.RemoveComponent<ResourceTargetPosition>(entityInQueryIndex, e);
            }

        }).ScheduleParallel();
        m_ECBSystem.AddJobHandleForProducer(Dependency);
        // get each entity that is in the WorkerWaitingForDropOff and NextWorkerInLine has WorkerWaitingForPickup
    }
}

public struct BucketRef : IComponentData
{
    public Entity Bucket;
}

public class PassBucketSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        Entities.WithNone<WorkerMoveTo>()
            .ForEach((Entity e, in Worker target, in WorkerStartEndPositions positions, in BucketRef bucketRef) =>
            {
                if (target.NextWorkerInLine != default(Entity))
                {
                    bool nextWorkerIsMoving = HasComponent<WorkerMoveTo>(target.NextWorkerInLine);
                    bool nextWorkerHasBucket = HasComponent<BucketRef>(target.NextWorkerInLine);
                    if (!nextWorkerIsMoving && !nextWorkerHasBucket)
                    {
                        ecb.RemoveComponent<BucketRef>(e);
                        ecb.AddComponent(e, new WorkerMoveTo() { Value = positions.Start });

                        WorkerStartEndPositions nextDest = GetComponent<WorkerStartEndPositions>(target.NextWorkerInLine);
                        ecb.AddComponent(target.NextWorkerInLine, new WorkerMoveTo() { Value = nextDest.End });
                        ecb.AddComponent(target.NextWorkerInLine, bucketRef);
                        ecb.SetComponent(bucketRef.Bucket, new BucketWorkerRef() { WorkerRef = target.NextWorkerInLine });
                    }
                }
            }).Run();
        ecb.Playback(EntityManager);
        // get each entity that is in the WorkerWaitingForDropOff and NextWorkerInLine has WorkerWaitingForPickup
    }
}