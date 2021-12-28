using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace CombatBees.Testing.BeeFlight
{
    public partial class BeeTargetingSystemBuffer : SystemBase
    {
        // TODO: Don't assign resources that have been already brought home
        
        private Random random;
        
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonBeeMovement>();
            RequireSingletonForUpdate<BufferSingelton>();
            random = new Random(123);
        }

        protected override void OnUpdate()
        {
            // Get all resources
            EntityQuery resourcesQuery = GetEntityQuery(typeof(Resource));
            NativeArray<Entity> allResourceEntities = resourcesQuery.ToEntityArray(Allocator.TempJob);
            
            var pairBuffer = GetBuffer<BeeResourcePair>(GetSingletonEntity<BufferSingelton>());
            var resourceBuffer = GetBuffer<ResourceBuffer>(GetSingletonEntity<BufferSingelton>());
            
            // Copy resources from a Native Array to a List (for easy removal of items)
            // List<Entity> resourceEntitiesList = new List<Entity>();
            //
            // foreach (var resourceEntity in allResourceEntities)
            // {
            //     resourceEntitiesList.Add(resourceEntity);
            // }
            
            // Initialize a random resource variable
            // Entity randomResource = Entity.Null;

            
            // List<Entity> assignedResources = new List<Entity>();
            // //initializing pair buffer 
            // Entities.WithAll<Bee>().ForEach((Entity entity) =>
            // {
            //     Enabled = false;
            //     pairBuffer.Add(new BeeResourcePair
            //     {   
            //         ResourceEntity = Entity.Null,
            //         BeeEntity = entity,
            //         
            //     });
            //
            // }).WithoutBurst().Run();
            
            //initializing resource buffer 
            // Entities.WithAll<Resource>().ForEach((Entity entity) =>
            // {
            //     Enabled = false;
            //     resourceBuffer.Add(new ResourceBuffer()
            //     {   
            //         Value = entity
            //     });
            //
            // }).WithoutBurst().Run();
            
            int k = 0;
            Entities.WithAll<Bee>().ForEach((Entity entity,ref IsHoldingResource isHoldingResource) =>
            {
                
                for(int i=0;i<pairBuffer.Length;i++)
                {
                    if (pairBuffer[i].BeeEntity == entity)
                    {
                        
                        if (pairBuffer[i].ResourceEntity == Entity.Null && resourceBuffer.Length > 0)
                        {
                            int randomIndex = random.NextInt(resourceBuffer.Length);
                            var selectedResource = resourceBuffer[randomIndex];
                            resourceBuffer.RemoveAt(randomIndex);
                            //should remove the bees without resource here 
                            // pairBuffer.RemoveAt(i);
                            pairBuffer.Add(new BeeResourcePair
                            {
                                ResourceEntity = selectedResource.Value,
                                BeeEntity = pairBuffer[i].BeeEntity,
                                index = k

                            });
                            pairBuffer.RemoveAt(i);
                        }
                        if (isHoldingResource.ReachedHome)
                        {
                            
                            pairBuffer.Add(new BeeResourcePair
                            {
                                ResourceEntity = Entity.Null,
                                BeeEntity = pairBuffer[i].BeeEntity,
                                index = k
                        
                            });
                            pairBuffer.RemoveAt(i);
                            
                        }
                    }

                }
              
              

            }).WithoutBurst().Run();
            
            // Entities.WithAll<Bee>().ForEach((Entity entity, ref BeeTargets beeTargets) =>
            // {
            //     if (beeTargets.ResourceTarget != Entity.Null)
            //     {
            //         // Remove the already assigned resources from the list
            //         resourceEntitiesList.Remove(beeTargets.ResourceTarget);
            //     }
            // }).WithoutBurst().Run();
            //
            // Entities.WithAll<Bee>().ForEach((Entity entity, ref BeeTargets beeTargets) =>
            // {
            //     if (beeTargets.ResourceTarget == Entity.Null && resourceEntitiesList.Count > 0)
            //     {
            //         // Pick a random unassigned resource and assign it to a bee
            //         int randomIndex = random.NextInt(resourceEntitiesList.Count);
            //         randomResource = resourceEntitiesList[randomIndex];
            //         beeTargets.ResourceTarget = randomResource;
            //         resourceEntitiesList.RemoveAt(randomIndex);
            //     }
            //     // equivalent of above with buffer :
            //     // buffer is empty in first run 
            //
            //     // foreach (var pair in  pairBuffer)
            //     // {
            //     //     if (pair.ResourceEntity == Entity.Null)
            //     //     {
            //     //         
            //     //     }
            //     // }
            //     // int randomIndex = random.NextInt(allResourceEntities.Length);
            //     // //before adding need to check if the resource has already been assigned or not 
            //     // buffer.Add(new BeeResourcePair
            //     // {
            //     //     ResourceEntity =allResourceEntities[randomIndex],
            //     //          
            //     // });
            //     
            //     
            // }).WithoutBurst().Run();
            // if we use ScheduleParallel(), we can't write to variables defined outside of EFE
            
            // if we remove WithoutBurst(), we can't use "List<Entity>" (if we use "NativeList<Entity>" we get error
            // "The UNKNOWN_OBJECT_TYPE has been deallocated"

            allResourceEntities.Dispose();
        }
    }
}