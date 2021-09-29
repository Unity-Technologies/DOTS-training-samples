using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class DroppedFoodSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var system = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = system.CreateCommandBuffer().AsParallelWriter();
        Prefabs prefabs = GetSingleton<Prefabs>();
        WorldBounds bounds = GetSingleton<WorldBounds>();
        var particleArchetype = EntityManager.CreateArchetype(typeof(ParticleSpawner));
        float3 direction = new float3(1.0f, 0.0f, 0.0f);

        Entities
            .WithAll<Food, Grounded, InHive>()
            .ForEach((Entity entity, int entityInQueryIndex, in Translation translation) =>
            {
                // destroy food
                ecb.DestroyEntity(entityInQueryIndex, entity);
                
                // spawn cloud particles
                var spawnCloudEntity = ecb.CreateEntity(entityInQueryIndex, particleArchetype);
                var spawnCloudSpawner = new ParticleSpawner
                {
                    Prefab = prefabs.SpawnCloudPrefab,
                    Position = translation.Value,
                    Direction = direction,
                    Spread = 0.2f,
                    Lifetime = 5.0f,
                    Count = 5,
                };
                ecb.SetComponent(entityInQueryIndex, spawnCloudEntity, spawnCloudSpawner);
                ecb.AddComponent<Grounded>(entityInQueryIndex, spawnCloudEntity);
                
                // spawn bees
                Entity prefab = prefabs.RedBeePrefab;

                if (WorldUtils.IsInBlueHive(bounds, translation.Value))
                {
                    prefab = prefabs.BlueBeePrefab;
                }
                
                var instance = ecb.Instantiate(entityInQueryIndex, prefab);
                var position = new Translation { Value = translation.Value };
                ecb.SetComponent(entityInQueryIndex, instance, position);
            }).ScheduleParallel();
        
        system.AddJobHandleForProducer(Dependency);
    }
}