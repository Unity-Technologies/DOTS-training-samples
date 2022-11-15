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
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var localToWorld in SystemAPI.Query<RefRW<LocalToWorldTransform>>().WithAll<Player>())
        {
            localToWorld.ValueRW.Value.Position += 
                new float3(playerInput.movement.x, 0, playerInput.movement.y) * SystemAPI.Time.DeltaTime;
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}