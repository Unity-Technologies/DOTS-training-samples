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
using UnityEngine.UIElements;

namespace Combatbees.Testing.Mahmoud
{
    public partial class SpawnResources : SystemBase
    {

        public static bool spawn;
        private int j = 0;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonHybridSpawner>();
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
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        var instance = ecb.Instantiate(resourceComponent.resourcePrefab);
                        //j++;
                        if (CameraRay.isMouseTouchingField)
                        {
                            var translation = new Translation {Value = CameraRay.worldMousePosition};
                            ecb.SetComponent(instance, translation);
                        }
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
                }).WithoutBurst().Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}