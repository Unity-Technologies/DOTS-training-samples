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
using Random = Unity.Mathematics.Random;

namespace Combatbees.Testing.Elfi
{
    public partial class ESpawnResources : SystemBase
    {

        // public static bool spawn;
        private int j = 0;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<ESingeltonHybridSpawner>();
            
        }

        
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            //var random = new Unity.Mathematics.Random((uint)UnityEngine.Random.Range(-10, 10));

            Entities
                .ForEach((Entity entity, ref EResourceComponent resourceComponent) =>
                {

                    for (int i = 0; i < resourceComponent.startResourceCount; i++)
                    {
                        var instance = ecb.Instantiate(resourceComponent.resourcePrefab);
                        var translation = new Translation();
                        translation.Value = new float3(
                            resourceComponent.random.NextInt(-20, 20) * resourceComponent.gridX,
                            20,
                            resourceComponent.random.NextInt(-5, 5)) * resourceComponent.gridZ;
                        ecb.SetComponent(instance, translation);
                        ecb.RemoveComponent<EResourceComponent>(entity);
                        resourceComponent.spawnedResources += 1;
                        //return resourceComponent.spawnedResources;
                    }
                }).WithoutBurst().Run();
            
                    
                    //not ECS Version: void SpawnResource() {Vector3 pos = new Vector3(minGridPos.x * .25f + Random.value * Field.size.x * .25f,
                    //Random.value * 10f,minGridPos.y + Random.value * Field.size.z);
                    //SpawnResource(pos);}
                    
           /* Entities
                .ForEach((Entity entity, ref EResourceComponent resourceComponent) =>
                { 
                    if (resourceComponent.spawnedResources<resourceComponent.startResourceCount)
                    {
                        var instance = ecb.Instantiate(resourceComponent.resourcePrefab);
                        var translation = new Translation();
                        translation.Value = new float3(
                            resourceComponent.random.NextInt(-5,5)* resourceComponent.gridX,
                            20,
                            resourceComponent.random.NextInt(-5,5))* resourceComponent.gridZ;
                        ecb.SetComponent(instance, translation);
                        ecb.RemoveComponent<EResourceComponent>(entity);
                        resourceComponent.spawnedResources += 1;
                    }
                
                        
                }).WithoutBurst().Run();*/
            
            
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }   
    }
}