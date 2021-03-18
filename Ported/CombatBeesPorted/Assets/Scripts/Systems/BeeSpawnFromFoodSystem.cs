using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class BeeSpawnFromFoodSystem : SystemBase
{
    private EntityQuery boundsQuery;
    protected override void OnCreate()
    {
        var boundsQueryDesc= new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(SpawnBounds)},
            Any = new ComponentType[] {typeof(TeamA),typeof(TeamB)}
        };
        boundsQuery = GetEntityQuery(boundsQueryDesc);
    }

    protected override void OnUpdate()
    {
        var gameConfig = GetSingleton<GameConfiguration>();
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var spawnBoundsArray = boundsQuery.ToEntityArray(Allocator.Temp);
        Entities
            .WithChangeFilter<Grounded>()
            .ForEach((Entity entity,in Translation translation,in Food food,in Grounded grounded) =>
            {
                if (GetComponent<SpawnBounds>(spawnBoundsArray[0]).ContainsPoint(new float3(translation.Value.x,translation.Value.y ,translation.Value.z)))
                {
                    var beeSpawnerEntity = ecb.CreateEntity();
                    ecb.AddComponent(beeSpawnerEntity, new BeeSpawnConfiguration() { Count = gameConfig.BeeSpawnPerCollectedFood });
                    ecb.AddComponent(beeSpawnerEntity, new Translation() { Value = translation.Value + new float3(0, 0.5f, 0) });
               
                    ecb.AddComponent(beeSpawnerEntity, new TeamB());
                    ecb.DestroyEntity(entity);
                    
                    ecb.AddComponent<DustSpawnConfiguration>(beeSpawnerEntity, new DustSpawnConfiguration() { Count = 5, Direction =  new float3(1f,1f,1f)});
                }

                else if (GetComponent<SpawnBounds>(spawnBoundsArray[1]).ContainsPoint(new float3(translation.Value.x,translation.Value.y ,translation.Value.z)))
                {
                    var beeSpawnerEntity = ecb.CreateEntity();
                    ecb.AddComponent(beeSpawnerEntity, new BeeSpawnConfiguration() { Count = gameConfig.BeeSpawnPerCollectedFood });
                    ecb.AddComponent(beeSpawnerEntity, new Translation() { Value = translation.Value + new float3(0, 0.5f, 0) });

                    ecb.AddComponent(beeSpawnerEntity, new TeamA());
                    ecb.DestroyEntity(entity);
                    
                    ecb.AddComponent<DustSpawnConfiguration>(beeSpawnerEntity, new DustSpawnConfiguration() { Count = 5, Direction =  new float3(1f,1f,1f)});
                }
            }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
