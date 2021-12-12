using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.CodeGeneratedJobForEach;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public partial class SpawnResources : SystemBase
{
    private bool spawn;
    private int j = 0;
    protected override void OnCreate()
    {
       RequireSingletonForUpdate<SingeltonSpawner>();
       spawn = false;

    }

    
    protected override void OnUpdate()
    {
       var ecb = new EntityCommandBuffer(Allocator.Temp);

       // if (Input.GetKeyDown(KeyCode.Mouse0))
       // {
       //     spawn = true;
       // }
        Entities
            .ForEach((Entity entity, in ResourceComponent resourceComponent) =>
            {
               // ecb.DestroyEntity(entity);
                if (Input.GetKey(KeyCode.I))
                {
                    var instance = ecb.Instantiate(resourceComponent.resourcePrefab);
                    //j++;
                    var translation = new Translation {Value = new float3(Input.mousePosition.x,0,Input.mousePosition.z)};
                    ecb.SetComponent(instance, translation);
                    
                  //  spawn = false;
                }
                // {
                //    // Debug.Log("sss");
                //     var instance = ecb.Instantiate(resourceComponent.resourcePrefab);
                //     j++;
                //     var translation = new Translation {Value = new float3(Input.mousePosition.x,0,Input.mousePosition.z)};
                //     ecb.SetComponent(instance, translation);
                //     spawn = false;
                // // }
                // // }
                // // }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}