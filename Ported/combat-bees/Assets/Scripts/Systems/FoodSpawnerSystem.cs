using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct FoodSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<BeeConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BeeConfig>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

#if false
        // This system will only run once, so the random seed can be hard-coded.
        // Using an arbitrary constant seed makes the behavior deterministic.
        var random = Random.CreateFromIndex(1234);
        
        float radius = 10.0f;

        var foodArray = CollectionHelper.CreateNativeArray<Entity>(config.foodCount, Allocator.Temp);
        ecb.Instantiate(config.food, foodArray);

        foreach (var food in foodArray)
        {
            var pos = random.NextFloat3();
            pos *= radius;
            var tm = state.EntityManager.GetComponentData<LocalToWorldTransform>(config.food);
            tm.Value.Position = pos;
            ecb.SetComponent(food, tm);
        }
#else
        var foodSpawnJob = new SpawnJob
        {
            // Note the function call required to get a parallel writer for an EntityCommandBuffer.
            Aabb = new AABB()
            {
                // Todo : Food should use a neutral area to spawn, not hardcoded like that.
                Center = new float3(0f, 0f, 0f),
                Extents = new float3(5f, 5f, 5f)
            },
            
            ECB = ecb.AsParallelWriter(),
            Prefab = config.food,
            InitFaction = (int)Factions.None,
        };

        // establish dependency between the spawn job and the command buffer to ensure the spawn job is completed
        // before the command buffer is played back.
        state.Dependency = foodSpawnJob.Schedule(config.foodCount, 64, state.Dependency);
        
        // Note: use CombineDependencies in order to combine Jobs into a single node for the dependency graph,
        // to ensure they can be executed in parallel and simultaneously.
        // JobHandle.CombineDependencies(...)
#endif
        // force disable system after the first update call
        state.Enabled = false;
    }
}