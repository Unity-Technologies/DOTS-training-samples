using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class TargetSpawnerSystem: JobComponentSystem
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
        
        Entities.WithStructuralChanges().ForEach((Entity entity, ref TargetSpawnerComponentData spawner) =>
        {
            spawner.spawnRemaining -= Time.DeltaTime;
            if (spawner.spawnRemaining < 0.0f)
            {
                spawner.spawnRemaining = spawner.spawnFrequency;
                    
              
                    
                var spawnX = rng.NextFloat(spawner.xRange.x, spawner.xRange.y);
                var spawnY = rng.NextFloat(spawner.yRange.x, spawner.yRange.y);
                
                Entity projectile = ecb.Instantiate(spawner.prefab);
                
                ecb.SetComponent(projectile, new Translation()
                {
                    Value = new float3(spawnX,spawnY,spawner.spawnZ)
                });
                
                var data = new TargetComponentData()
                {
                    velocityX = spawner.velocityX,
                    rangeXMin = spawner.xRange.x,
                    rangeXMax = spawner.xRange.y,
                };
                
                ecb.AddComponent(projectile,data);
                    
                    
            }
        }).Run();
        
        return inputDeps;
    }
}
