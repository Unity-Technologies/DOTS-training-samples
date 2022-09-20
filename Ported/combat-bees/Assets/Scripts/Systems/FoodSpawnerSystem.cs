using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


// [BurstCompile]
// partial struct SpawnJob : IJobEntity
// {
//     // A regular EntityCommandBuffer cannot be used in parallel, a ParallelWriter has to be explicitly used.
//     public EntityCommandBuffer.ParallelWriter ECB;
//     
//     public Entity Prefab;
//     public int Count;
//
//     // The ChunkIndexInQuery attributes maps the chunk index to an int parameter.
//     // Each chunk can only be processed by a single thread, so those indices are unique to each thread.
//     // They are also fully deterministic, regardless of the amounts of parallel processing happening.
//     // So those indices are used as a sorting key when recording commands in the EntityCommandBuffer,
//     // this way we ensure that the playback of commands is always deterministic.
//     void Execute([ChunkIndexInQuery] int chunkIndex)
//     {
//         var entity = ECB.Instantiate(chunkIndex, Prefab);
//         
//         // This system will only run once, so the random seed can be hard-coded.
//         // Using an arbitrary constant seed makes the behavior deterministic.
//         //var random = Random.CreateFromIndex((uint)chunkIndex);
//         var random = Random.
//
//         float radius = 10.0f;
//
//         var food = ecb.Instantiate(Prefab);
//             
//             var pos = random.NextFloat3();
//             pos *= radius;
//             var tm = SystemAPI.GetAspectRW<TransformAspect>(food);
//             tm.Position = pos;
//         }
//     }
// }
    
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
        
        // This system will only run once, so the random seed can be hard-coded.
        // Using an arbitrary constant seed makes the behavior deterministic.
        var random = Random.CreateFromIndex(1234);
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        float radius = 10.0f;
        
        var foodArray = CollectionHelper.CreateNativeArray<Entity>(config.foodCount, Allocator.Temp);
        ecb.Instantiate(config.food, foodArray);

        foreach (var food in foodArray)
        {
            var pos = random.NextFloat3();
            pos *= radius;
            var tm = state.EntityManager.GetComponentData<LocalToWorldTransform>(config.food);
            tm.Value.Position = pos;
            ecb.SetComponent<LocalToWorldTransform>(food, tm);
        }

        // for (int i = 0; i < config.foodCount; ++i)
        // {
        //     var food = ecb.Instantiate(config.food);
        //     
        //     var pos = random.NextFloat3();
        //     pos *= radius;
        //     var tm = SystemAPI.GetAspectRW<TransformAspect>(food);
        //     tm.Position = pos;
        // }
        
        // var foodSpawnJob = new SpawnJob
        // {
        //     // Note the function call required to get a parallel writer for an EntityCommandBuffer.
        //     ECB = ecb.AsParallelWriter(),
        //     Prefab = config.food,
        //     Count = config.foodCount 
        // };
        // foodSpawnJob.ScheduleParallel();
        
        // force disable system after the first update call
        state.Enabled = false;
    }
}