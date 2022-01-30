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
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities.WithAll<ResourceTag>().ForEach((Entity entity,ref Translation translation) => {
    
            if (translation.Value.x <- 40&& translation.Value.y<-9.5)
            {
                for (int i = 0; i < spawnerDataTeamA.SpawnedCount; i++)
                {
                    var prefabInstance = ecb.Instantiate(spawnerDataTeamA.PrefabToSpawn);
    
                    float3 velocity = spawnerDataTeamA.Random.NextFloat3(
                        new float3(-1, -1, -1) * spawnerDataTeamA.MaxInitVelocity,
                        new float3(1, 1, 1) * spawnerDataTeamA.MaxInitVelocity);
    
                    ecb.SetComponent(prefabInstance, translation);
                    ecb.SetComponent(prefabInstance, new Velocity {Value = velocity});
                }
    
                ecb.DestroyEntity(entity);
            } 
            if (translation.Value.x > 40&& translation.Value.y<-9.5)
            {
                for (int i = 0; i < spawnerDataTeamB.SpawnedCount; i++)
                {
                    var prefabInstance = ecb.Instantiate(spawnerDataTeamB.PrefabToSpawn);
    
                    float3 velocity = spawnerDataTeamB.Random.NextFloat3(
                        new float3(-1, -1, -1) * spawnerDataTeamB.MaxInitVelocity,
                        new float3(1, 1, 1) * spawnerDataTeamB.MaxInitVelocity);
    
                    ecb.SetComponent(prefabInstance, translation);
                    ecb.SetComponent(prefabInstance, new Velocity {Value = velocity});
                }
    
                ecb.DestroyEntity(entity);
            }
    
        }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
