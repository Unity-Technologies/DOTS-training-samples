using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct FoodSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<Config>(); //Needed to be able to get singletons on first frame
        state.RequireForUpdate<FoodSpawnerComponent>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false; //do not update again
        var config = SystemAPI.GetSingleton<Config>(); // does not exist at first frame if not required
        var foodSpawner = SystemAPI.GetSingleton<FoodSpawnerComponent>();

        var random = new Random(123); //we could give time as a seed

        for (int i = 0; i < config.foodCount; i++)
        {
            Entity newFood = state.EntityManager.Instantiate(foodSpawner.foodPrefab);
            float2 spawnBoundaries = random.NextFloat2(config.foodBounds) - config.foodBounds/2;

            var position = new float3(spawnBoundaries.x, 0, spawnBoundaries.y);
            state.EntityManager.SetComponentData<LocalTransform>(newFood, new LocalTransform { 
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1.0f
            });
            //LocalTransform.FromPosition(position);
        }

        // spawn new bees when food is placed in hive
        // remove food that has been placed
    }
}