using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct ResourceSpawningSystem : ISystem
{
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<ResourceConfig>(); // TODO do we still want a singleton?
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<ResourceConfig>();
        
        // This system will only run once, so the random seed can be hard-coded.
        // Using an arbitrary constant seed makes the behavior deterministic.
        var random = Random.CreateFromIndex(1234);
 
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var initialSpawnJob = new SpawnResourceJob()
        {
            ECB = ecb,
            ResourcePrefab = config.ResourcePrefab,
            SpawnCount = config.InitialCount,
            SpawnRadius = config.SpawnRadius,
            Random = random
        };
        
        var spawnHandle = initialSpawnJob.Schedule();
        spawnHandle.Complete();
        
        state.Enabled = false;
    }
    
    // TODO spawn on click too
    [BurstCompile]
    struct SpawnResourceJob : IJob
    {
        public EntityCommandBuffer ECB;
        public Entity ResourcePrefab;
        public int SpawnCount;
        public Random Random;
        public float SpawnRadius;
    
        public void Execute()
        {
            var resources = CollectionHelper.CreateNativeArray<Entity>(SpawnCount, Allocator.Temp);
            ECB.Instantiate(ResourcePrefab, resources);

            foreach (var resource in resources)
            {
                var uniformScaleTransform = new UniformScaleTransform
                {
                    // ALX: now using Field bounds, but could cluster closer to the centre if desired
                    // Position = Random.NextFloat3(new float3(-SpawnRadius, 0, -SpawnRadius), new float3(SpawnRadius, 0, SpawnRadius)),
                    Position = Random.NextFloat3(Field.ResourceBoundsMin, Field.ResourceBoundsMax),
                    Rotation = quaternion.identity,
                    Scale = 1
                };
                
                // Set object position to position
                ECB.SetComponent(resource, new LocalToWorldTransform
                {
                    Value = uniformScaleTransform
                });

                // Start off with an alive resource
                ECB.AddComponent(resource, new Resource()
                {
                    Dead = false
                });

                // Set component data to position
                ECB.AddComponent(resource, new Physical()
                {
                    Position = uniformScaleTransform.Position
                });
            }
        }
    }
}