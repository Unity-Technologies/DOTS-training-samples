using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


[BurstCompile]
struct SpawnJob : IJobParallelFor
{
    // A regular EntityCommandBuffer cannot be used in parallel, a ParallelWriter has to be explicitly used.
    public EntityCommandBuffer.ParallelWriter ECB;
    
    public Entity Prefab;

    public LocalToWorldTransform InitTM;

    public void Execute(int index)
    {
        var entity = ECB.Instantiate(index, Prefab);
        // This system will only run once, so the random seed can be hard-coded.
        // Using an arbitrary constant seed makes the behavior deterministic.
        //var random = Random.CreateFromIndex((uint)chunkIndex);
        var random = Random.CreateFromIndex((uint) index);

        float radius = 10.0f;
        var pos = random.NextFloat3();
        pos *= radius;

        var tm = InitTM;
        tm.Value.Position = pos;
        ECB.SetComponent(index, entity, tm);
    }
}
    
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
            ECB = ecb.AsParallelWriter(),
            Prefab = config.food,
            InitTM = state.EntityManager.GetComponentData<LocalToWorldTransform>(config.food)
        };

        JobHandle jobHandle = foodSpawnJob.Schedule(config.foodCount, 64);
        jobHandle.Complete();
#endif
        // force disable system after the first update call
        state.Enabled = false;
    }
}