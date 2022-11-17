using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PlayerMovementSystem))]
[BurstCompile]
public partial struct PillPickupSystem : ISystem
{
    private SystemHandle m_PillSpawnSystemHandle;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Player>();
        state.RequireForUpdate<Pill>();
        m_PillSpawnSystemHandle = state.WorldUnmanaged.GetExistingUnmanagedSystem<PillSpawnSystem>();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerRadius = 0.4f; // TODO: should get from player mesh half-size
        var playerEntity = SystemAPI.GetSingletonEntity<Player>();
        var gameConfig = SystemAPI.GetSingleton<GameConfig>();
        var gameConfigEntity = SystemAPI.GetSingletonEntity<GameConfig>();

        var ltwLookup = SystemAPI.GetComponentLookup<LocalToWorldTransform>();
        var playerPos = ltwLookup[playerEntity].Value.Position;
        
        ref var pillSpawnSystemState = ref state.WorldUnmanaged.ResolveSystemStateRef(m_PillSpawnSystemHandle);
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (pillLTW, pillEntity) in SystemAPI.Query<LocalToWorldTransform>().WithAll<Pill>()
            .WithEntityAccess())
        {
            if (math.distancesq(playerPos, pillLTW.Value.Position) < playerRadius * playerRadius)
            {
                ecb.DestroyEntity(pillEntity);

                gameConfig.gameState.score += 1;
                if (gameConfig.gameState.score >= gameConfig.numPills)
                {
                    Debug.Log("You win!");
                    gameConfig.gameState.score = 0;
                    pillSpawnSystemState.Enabled = true;
                }
                
                ecb.SetComponent(gameConfigEntity, gameConfig);
                // TODO: score++ and check if score == totalPills for game win
            }
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}