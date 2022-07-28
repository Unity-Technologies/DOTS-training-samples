using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct BloodSystem : ISystem
{
    private Unity.Mathematics.Random random;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var dt = Time.deltaTime;
        random = Random.CreateFromIndex(state.GlobalSystemVersion);
        foreach (var (blood, translation, scale)
            in SystemAPI.Query<Entity, RefRW<Translation>, RefRW<NonUniformScale>>().WithAll<Blood>())
        {
            if (translation.ValueRO.Value.y >= -9.5f)
            {
                translation.ValueRW.Value += new float3(0,-9.82f * 3,0) * dt;
            }
            else
            {
                scale.ValueRW.Value += new float3(10, -4, 10) * dt;
                if (scale.ValueRO.Value.y <= 0)
                {
                    ecb.DestroyEntity(blood);
                }
            }
            
            
            

        }
    }
}