using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class SpawnResources : SystemBase
{
    //List<Resource> resources;
    // private List<Matrix4x4> matrices;
    // use render mesh utility to add mesh? 
    protected override void OnCreate()
    {
       RequireSingletonForUpdate<SingeltonSpawner>();
    }

    protected override void OnUpdate()
    {
       // var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.WithStructuralChanges()
            .ForEach((Entity entity, in ResourceComponent resourceComponent) =>
            {
                //ecb.DestroyEntity(entity);

                // for (int i = 0; i < resourceComponent.gridX; i++)
                // {
                    //Debug.Log("sss");
                    // for (int j = 0; j < resourceComponent.gridY; j++)
                    // {
                    var instance = EntityManager.Instantiate(resourceComponent.resourcePrefab);
                    // var translation = new Translation {Value = new float3(0, 0, 4)};
                    // EntityManager.SetComponentData(instance, translation);
                    // }
                // }
            }).Run();

        /*ecb.Playback(EntityManager);
        ecb.Dispose();*/
    }
}