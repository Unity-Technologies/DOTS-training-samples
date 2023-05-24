using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct FoodSystem : ISystem
{
    private uint _updateCounter;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var droppedFoods = new NativeList<DroppedFood>(Allocator.Temp);
        FindDroppedFoods(ref droppedFoods, ref config, ref state);
        ProcessDroppedFoods(droppedFoods, ref config, ref state);
        droppedFoods.Dispose();
    }

    [BurstCompile]
    private void FindDroppedFoods(
        ref NativeList<DroppedFood> droppedFoods,
        ref Config config, 
        ref SystemState state)
    {
        foreach (var (foodTransform, foodEntity) in SystemAPI
            .Query<RefRO<LocalTransform>>()
            .WithAll<FoodComponent>()
            .WithEntityAccess())
        {
            if (foodTransform.ValueRO.Position.y <= -config.bounds.y)
            {
                droppedFoods.Add(new DroppedFood
                {
                    FoodEntity = foodEntity,
                    Position = foodTransform.ValueRO.Position
                });
            }
        }
    }

    [BurstCompile]
    private void ProcessDroppedFoods(
        NativeList<DroppedFood> droppedFoods,
        ref Config config,
        ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var random = Random.CreateFromIndex(_updateCounter);

        foreach (var spawner in SystemAPI.Query<RefRO<BeeSpawnerComponent>>())
        {
            float3 minBounds = spawner.ValueRO.minBounds;
            float3 maxBounds = spawner.ValueRO.maxBounds;

            for (int i = droppedFoods.Length - 1; i >= 0; --i)
            {
                var droppedFood = droppedFoods[i];

                if (droppedFood.Position.x < minBounds.x
                    || droppedFood.Position.x > maxBounds.x
                    || droppedFood.Position.y < minBounds.y
                    || droppedFood.Position.y > maxBounds.y
                    || droppedFood.Position.z < minBounds.z
                    || droppedFood.Position.z > maxBounds.z)
                {
                    continue;
                }

                BeeSpawnerSystem.SpawnBees(
                    config.respawnBeeCount,
                    spawner.ValueRO.beePrefab,
                    droppedFood.Position,
                    config.maxSpawnSpeed,
                    spawner.ValueRO.hiveId,
                    spawner.ValueRO.minBounds,
                    spawner.ValueRO.maxBounds,
                    ecb,
                    ref random);

                _updateCounter += (uint)config.respawnBeeCount;

                ecb.DestroyEntity(droppedFood.FoodEntity);
                droppedFoods.RemoveAt(i);
            }

            if (droppedFoods.IsEmpty)
            {
                break;
            }
        }
    }

    private struct DroppedFood
    {
        public Entity FoodEntity;
        public float3 Position;
    }
}
