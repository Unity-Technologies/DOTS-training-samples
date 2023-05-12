using Components;
using Metro;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(PassengerSystemGroup))]
[UpdateAfter(typeof(PassengerWalkingToSeatSystem))]
public partial struct PassengerWalkingToDoorSystem : ISystem
{
    public float3 Y;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
        state.RequireForUpdate<StationConfig>();

        Y = new float3(0, 1f, 0);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        
        var movePassengerToExitJobJob = new MovePassengerToExitJob
        {
            PassengerSpeed = SystemAPI.Time.DeltaTime * config.PassengerSpeed,
            Y = Y,
            Ecb = ecb.AsParallelWriter()
        };
        
        state.Dependency = movePassengerToExitJobJob.ScheduleParallel(state.Dependency);

    }
}

[WithAll(typeof(PassengerWalkingToDoor))]
[BurstCompile]
public partial struct MovePassengerToExitJob : IJobEntity
{
    public float PassengerSpeed;
    public float3 Y;
    public EntityCommandBuffer.ParallelWriter Ecb;
    
    public void Execute([ChunkIndexInQuery] int chunkIndex, ref LocalTransform transform, ref PassengerComponent passengerInfo, Entity passengerEntity)
    {
        var toDst = passengerInfo.ExitPosition - passengerInfo.RelativePosition;
        var dist = math.length(toDst);
        
        if (dist < 0.1f)
        {
            Ecb.SetComponentEnabled<PassengerWalkingToDoor>(chunkIndex, passengerEntity, false);
            Ecb.SetComponentEnabled<PassengerWaitingToExit>(chunkIndex, passengerEntity, true);
        }
        else
        {
            var toDstDirection = math.normalize(toDst);
            passengerInfo.RelativePosition += toDstDirection * PassengerSpeed;
        
            var rotationAngle = math.acos(math.dot(Y, toDstDirection));
            transform.Rotation = quaternion.RotateY(rotationAngle);
        }
    }
}
