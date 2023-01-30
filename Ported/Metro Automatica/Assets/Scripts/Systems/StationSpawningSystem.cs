using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
partial struct StationSpawningSystem : ISystem
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

        var stations = CollectionHelper.CreateNativeArray<Entity>(config.StationCount, Allocator.Temp);
        ecb.Instantiate(config.StationPrefab, stations);

        for (int i = 0; i < stations.Length; i++)
        {
            var transform = LocalTransform.FromPosition(i * 90, 0, 0);
          //  transform.Rotation = quaternion.RotateY(math.radians(90));
            ecb.SetComponent(stations[i], transform);
        }

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}
