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
            All = new ComponentType[] {typeof(Bounds2D)},
            Any = new ComponentType[] {typeof(TeamA),typeof(TeamB)}
        };
        boundsQuery = GetEntityQuery(boundsQueryDesc);

    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        var spawnBoundsArray = boundsQuery.ToEntityArray(Allocator.Temp);

        Entities
            .WithChangeFilter<Grounded>()
            .ForEach((Entity entity,in Translation translation,in Food food,in Grounded grounded) =>
            {

                if (GetComponent<Bounds2D>(spawnBoundsArray[0]).Contains(new float2(translation.Value.x, translation.Value.z)))
                {
                    var beeSpawnerEntity = ecb.CreateEntity();
                    ecb.AddComponent(beeSpawnerEntity, new BeeSpawnConfiguration() { Count = 5 });
                    ecb.AddComponent(beeSpawnerEntity, new Translation() { Value = translation.Value + new float3(0, 0.5f, 0) });
               
                    ecb.AddComponent(beeSpawnerEntity, new TeamB());
                    ecb.DestroyEntity(entity);

                }

                else if (GetComponent<Bounds2D>(spawnBoundsArray[1]).Contains(new float2(translation.Value.x, translation.Value.z)))
                {
                    var beeSpawnerEntity = ecb.CreateEntity();
                    ecb.AddComponent(beeSpawnerEntity, new BeeSpawnConfiguration() { Count = 5 });
                    ecb.AddComponent(beeSpawnerEntity, new Translation() { Value = translation.Value + new float3(0, 0.5f, 0) });

                    ecb.AddComponent(beeSpawnerEntity, new TeamA());
                    ecb.DestroyEntity(entity);
                }

                
            }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
