using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

partial struct RockSpawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<RockConfig>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var rocks = CollectionHelper.CreateNativeArray<Entity>(100, Allocator.Temp);
        ecb.Instantiate(config.RockPrefab, rocks);
        foreach (var rockTransform in SystemAPI.Query<RefRO<RockConfig>>())
        {
            for (int x = 0; x < rockTransform.ValueRO.NumRocks; x++)
            {
                var mapSize = new int2(100, 100);

                int width = UnityEngine.Random.Range(0, 100);
                int height = UnityEngine.Random.Range(0, 100);
                int rockX = UnityEngine.Random.Range(0, mapSize.x - width);
                int rockY = UnityEngine.Random.Range(0, mapSize.y - height);
                ecb.SetComponent(rocks[x], new Translation {Value = new float3(rockX, 0, rockY)});
            }
        }

        // foreach (var scale in SystemAPI.Query<RefRW<NonUniformScale>>().WithAll<RockTag>())
        // {
        //     int width = UnityEngine.Random.Range(1, 4);
        //     int length = UnityEngine.Random.Range(1, 4);
        //
        //     NonUniformScale s = new NonUniformScale { Value = new float3(width, 1, length) };
        //
        //     scale.ValueRW = s;
        // }

        state.Enabled = false;
    }
}