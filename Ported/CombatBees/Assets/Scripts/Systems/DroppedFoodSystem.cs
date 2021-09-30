using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class DroppedFoodSystem : SystemBase
{
    private Random random;
    protected override void OnCreate()
    {
        base.OnCreate();
        random = new Random((uint)System.DateTime.Now.Ticks);
    }
    
    protected override void OnUpdate()
    {
        var system = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = system.CreateCommandBuffer().AsParallelWriter();
        Prefabs prefabs = GetSingleton<Prefabs>();
        WorldBounds bounds = GetSingleton<WorldBounds>();
        Constants constants = GetSingleton<Constants>();
        var particleArchetype = EntityManager.CreateArchetype(typeof(ParticleSpawner));
        var seed = random.NextUInt();
        
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
                    Lifetime = 2.0f,
                    Speed = 5.0f,
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

                Random random = new Random(seed + (uint)entityInQueryIndex);
                uint numberToSpawn = random.NextUInt(constants.MinBeesToSpawnFromFood, constants.MaxBeesToSpawnFromFood);

                for (uint i = 0; i < numberToSpawn; ++i)
                {
                    var instance = ecb.Instantiate(entityInQueryIndex, prefab);
                    var position = new Translation { Value = translation.Value };
                    ecb.SetComponent(entityInQueryIndex, instance, position);            
                }
            }).ScheduleParallel();
        
        system.AddJobHandleForProducer(Dependency);
    }
}