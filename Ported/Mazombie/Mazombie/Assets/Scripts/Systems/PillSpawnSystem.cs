using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(MazeGeneratorSystem))]
[BurstCompile]
public partial struct PillSpawnSystem : ISystem
{
    private Random m_Random;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameConfig>();

    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameConfig = SystemAPI.GetSingleton<GameConfig>();
        if (m_Random.state == 0)
            m_Random.InitState(gameConfig.seed);
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        for (int i = 0; i < gameConfig.numPills; i++)
        {
            var gridPosX = m_Random.NextInt(0, gameConfig.mazeSize);
            var gridPosY = m_Random.NextInt(0, gameConfig.mazeSize);
            
            var pill = ecb.Instantiate(gameConfig.pillPrefab);
            ecb.SetComponent(pill, new LocalToWorldTransform
            {
                // Set rotation/scale here since it does not seem to get picked up from prefab..
                Value = UniformScaleTransform.FromPositionRotationScale(
                    MazeUtils.GridPositionToWorld(gridPosX, gridPosY),
                    quaternion.EulerXYZ(70, 70, 0),
                    0.3f
                    )
            });
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
        
        state.Enabled = false;
    }
}