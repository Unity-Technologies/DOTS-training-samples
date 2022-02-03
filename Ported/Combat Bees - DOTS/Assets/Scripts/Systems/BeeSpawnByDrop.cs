using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BeeSpawnByDrop : SystemBase
{
    private EntityCommandBufferSystem sys;

    protected override void OnCreate()
    {
        sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var spawnerDataTeamA = GetSingleton<BeesSpawnedAfterDropTeamA>();
        var spawnerDataTeamB = GetSingleton<BeesSpawnedAfterDropTeamB>();
        
        float floorHeight = GetSingleton<Container>().MinPosition.y;
        
        var ecb = sys.CreateCommandBuffer().AsParallelWriter();
        
        Entities.WithAll<ResourceTag>().ForEach((Entity entity,int entityInQueryIndex,ref Translation translation) => {
            if (translation.Value.x <- 40 && translation.Value.y <= floorHeight + 0.1f)
            {
                for (int i = 0; i < spawnerDataTeamA.SpawnedCount; i++)
                {
                    var prefabInstance = ecb.Instantiate(entityInQueryIndex,spawnerDataTeamA.PrefabToSpawn);
    
                    float3 velocity = spawnerDataTeamA.Random.NextFloat3(
                        new float3(-1, -1, -1) * spawnerDataTeamA.MaxInitVelocity,
                        new float3(1, 1, 1) * spawnerDataTeamA.MaxInitVelocity);
                    
                    ecb.SetComponent(entityInQueryIndex,prefabInstance, translation);
                    ecb.SetComponent(entityInQueryIndex,prefabInstance, new Velocity {Value = velocity});
                }
    
                ecb.DestroyEntity(entityInQueryIndex,entity);
            } 
            else if (translation.Value.x > 40 && translation.Value.y  <= floorHeight + 0.1f)
            {
                for (int i = 0; i < spawnerDataTeamB.SpawnedCount; i++)
                {
                    var prefabInstance = ecb.Instantiate(entityInQueryIndex,spawnerDataTeamB.PrefabToSpawn);
                    
                    float3 velocity = spawnerDataTeamB.Random.NextFloat3(
                        new float3(-1, -1, -1) * spawnerDataTeamB.MaxInitVelocity,
                        new float3(1, 1, 1) * spawnerDataTeamB.MaxInitVelocity);
    
                    ecb.SetComponent(entityInQueryIndex,prefabInstance, translation);
                    ecb.SetComponent(entityInQueryIndex,prefabInstance, new Velocity {Value = velocity});
                }
    
                ecb.DestroyEntity(entityInQueryIndex,entity);
            }
        
        }).Run();
        sys.AddJobHandleForProducer(this.Dependency);
    }
}
