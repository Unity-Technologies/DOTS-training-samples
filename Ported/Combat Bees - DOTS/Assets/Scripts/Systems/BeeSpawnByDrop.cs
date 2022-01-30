using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class BeeSpawnByDrop : SystemBase
{
    protected override void OnUpdate()
    {
        var spawnerDataTeamA = GetSingleton<BeesSpawnedAfterDropTeamA>();
        var spawnerDataTeamB = GetSingleton<BeesSpawnedAfterDropTeamB>();

        EntityCommandBufferSystem sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = sys.CreateCommandBuffer().AsParallelWriter();
        Entities.WithAll<ResourceTag>().ForEach((Entity entity,int entityInQueryIndex,ref Translation translation) => {
    
            if (translation.Value.x <- 40&& translation.Value.y<-9.5)
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
            if (translation.Value.x > 40&& translation.Value.y<-9.5)
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
    
        }).ScheduleParallel();
        sys.AddJobHandleForProducer(this.Dependency);
    }
}
