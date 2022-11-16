using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Player>();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerInput = SystemAPI.GetSingleton<PlayerInput>();
        var activeCamEntity = SystemAPI.GetSingletonEntity<ActiveCamera>();
        
        var ltwLookup = SystemAPI.GetComponentLookup<LocalToWorldTransform>();
        var cameraLTW = ltwLookup[activeCamEntity];
        
        float3 cameraFwd = math.mul(cameraLTW.Value.Rotation, math.forward());
        float3 cameraRight = math.mul(cameraLTW.Value.Rotation, math.right());
        float3 cameraForwardOnUpPlane = math.normalizesafe(cameraFwd - math.projectsafe(cameraFwd, math.up()));

        float3 moveVector = playerInput.movement.y * cameraForwardOnUpPlane + playerInput.movement.x * cameraRight;
        moveVector = math.normalizesafe(moveVector);
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var localToWorld in SystemAPI.Query<RefRW<LocalToWorldTransform>>().WithAll<Player>())
        {
            localToWorld.ValueRW.Value.Position += moveVector * SystemAPI.Time.DeltaTime;
            // todo: rotation resets when movevector == zero, keep track of this better
            localToWorld.ValueRW.Value.Rotation = quaternion.LookRotationSafe(moveVector, math.up());
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}