using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ProjectileSpawnerSystem : JobComponentSystem
{
    private BeginSimulationEntityCommandBufferSystem m_spawnerECB;
    private Random rng;
    
    protected override void OnCreate()
    {
        m_spawnerECB = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        
        rng = new Random();
        rng.InitState();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ecb = m_spawnerECB.CreateCommandBuffer();
        
        Entities.WithStructuralChanges().ForEach((Entity entity, ref ProjectileSpawnerComponentData spawner) =>
            {
                spawner.spawnRamaining -= Time.DeltaTime;
                if (spawner.spawnRamaining < 0.0f)
                {
                    spawner.spawnRamaining = spawner.spawnFrequency;
                    
                    var radius = rng.NextFloat(spawner.radiusRange.x,spawner.radiusRange.y);
                    var spawnX = rng.NextFloat(spawner.xRange.x, spawner.xRange.y);
                
                    Entity projectile = ecb.Instantiate(spawner.prefab);
                    ecb.AddComponent(projectile, new Scale
                    {
                        Value = radius * 2
                    });
                    ecb.SetComponent(projectile, new Translation()
                    {
                        Value = new float3(spawnX,0,spawner.spawnZ)
                    });
                
                    var data = new ProjectileComponentData()
                    {
                        radius = radius,
                        velocityX = spawner.velocityX,
                        rangeXMin = spawner.xRange.x,
                        rangeXMax = spawner.xRange.y
                    };
                
                    ecb.AddComponent(projectile,data);
                    
                    
                }
            }).Run();
        
        return inputDeps;
    }
}
