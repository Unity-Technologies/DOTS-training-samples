using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TargetSystem))]
[UpdateAfter(typeof(OmniWorkerAiSystem))]
[BurstCompile]
partial struct MovementSystem : ISystem
{
    // [ReadOnly] ComponentLookup<HasReachedDestinationTag> m_HasReachedDestinationTagLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // m_HasReachedDestinationTagLookup = state.GetComponentLookup<HasReachedDestinationTag>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // m_HasReachedDestinationTagLookup.Update(ref state);

        var config = SystemAPI.GetSingleton<Config>();

        var moveWorkerJob = new MoveWorkerJob()
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            workerSpeedConfig = config.workerSpeed,
            ECB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
        };
        state.Dependency = moveWorkerJob.Schedule(state.Dependency);
    }
}

// [WithNone(typeof(HasReachedDestinationTag))]
[BurstCompile]
partial struct MoveWorkerJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ECB;

    public float deltaTime;
    public float workerSpeedConfig;

    void Execute([ChunkIndexInQuery] int chunkIndex, in Entity entity, ref TransformAspect transform, ref Position position, in MoveInfo moveInfo, Target workerTarget)
    {
        var direction = moveInfo.destinationPosition - position.position;
        var distanceToDestination = math.length(direction);
        var distanceToTravel = deltaTime * workerSpeedConfig;
        // WaterAmountLookup.HasComponent(workerTarget.waterTargetEntity)
        if (distanceToTravel > distanceToDestination)
        {
            position.position = moveInfo.destinationPosition;
            ECB.SetComponentEnabled<HasReachedDestinationTag>(chunkIndex, entity, true); // should always interact with that tag at the end of the frame. Order of systems is a mess
        }
        else
        {
            position.position += distanceToTravel * direction / distanceToDestination;
            ECB.SetComponentEnabled<HasReachedDestinationTag>(chunkIndex, entity, false); // should always interact with that tag at the end of the frame. Order of systems is a mess
        }

        transform.LocalPosition = new float3(position.position.x, transform.LocalPosition.y, position.position.y);
    }
}
