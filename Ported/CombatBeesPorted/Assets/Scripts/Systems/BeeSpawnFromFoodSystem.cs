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
    private EntityQueryDesc boundsQuery;
    protected override void OnCreate()
    {
        boundsQuery= new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(SpawnBounds)},
            Any = new ComponentType[] {typeof(TeamA),typeof(TeamB)}
        };
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .WithChangeFilter<Grounded>()
            .ForEach((Entity entity,in Translation translation,in Food food,in Grounded grounded) =>
            {
                bool team =true;

                if (translation.Value.x > 40)
                    team = false;
                else if(translation.Value.x > -40)  
                    return;
                
                var beeSpawnerEntity = ecb.CreateEntity();
                ecb.AddComponent(beeSpawnerEntity,new BeeSpawnConfiguration(){Count = 5});
                ecb.AddComponent(beeSpawnerEntity,new Translation(){Value = translation.Value+new float3(0,0.5f,0)});
                if(team)
                    ecb.AddComponent(beeSpawnerEntity,new TeamA());
                else
                    ecb.AddComponent(beeSpawnerEntity,new TeamB());

                ecb.DestroyEntity(entity);
            }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
