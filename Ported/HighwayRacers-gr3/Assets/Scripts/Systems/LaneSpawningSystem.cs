using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
public partial struct LaneSpawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HighwayConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<HighwayConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        for (int i = 0; i < config.LaneCount; i++)
        {
            var laneEntity = ecb.CreateEntity();
            ecb.AddComponent<LaneTag>(laneEntity);
            ecb.AddComponent( laneEntity, new LaneComponent { LaneNumber = i + 1 });
            ecb.AddBuffer<CarElement>(laneEntity);
        }

        state.Enabled = false;
    }
}
