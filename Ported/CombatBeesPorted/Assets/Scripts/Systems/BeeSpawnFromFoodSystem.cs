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
    EndSimulationEntityCommandBufferSystem endSim;    

    protected override void OnCreate()
    {
        var boundsQueryDesc= new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(SpawnBounds)},
            Any = new ComponentType[] {typeof(TeamA),typeof(TeamB)}
        };
        boundsQuery = GetEntityQuery(boundsQueryDesc);
        endSim = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var gameConfig = GetSingleton<GameConfiguration>();
        var ecb = endSim.CreateCommandBuffer();
        var spawnBoundsArray = boundsQuery.ToEntityArray(Allocator.TempJob);
        Entities
            .WithChangeFilter<Grounded>()
            .WithDisposeOnCompletion(spawnBoundsArray)
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
            }).Schedule();
        endSim.AddJobHandleForProducer(Dependency);
    }
}
