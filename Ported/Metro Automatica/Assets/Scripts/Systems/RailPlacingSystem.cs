using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

[BurstCompile]
partial struct RailPlacingSystem : ISystem
{
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
        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        /*var railSpawn = ecb.Instantiate(config.RailPrefab);
        
        // need var distance /2 between first and last station in x value (for now)

        var transform = LocalTransform.FromPosition(45, 0, 0);
        
        // need var distance to calculate scale value

        ecb.SetComponent(railSpawn, new PostTransformScale{Value = float3x3.Scale(90,1,1)});

        ecb.SetComponent(railSpawn, transform); 
        
        // This system should only run once at startup. So it disables itself after one update.*/
        state.Enabled = false;
    }
}
