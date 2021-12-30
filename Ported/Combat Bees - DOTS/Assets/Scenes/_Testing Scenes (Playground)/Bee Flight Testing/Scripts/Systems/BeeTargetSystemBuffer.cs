using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace CombatBees.Testing.BeeFlight
{
    [UpdateBefore(typeof(BeeMovementSystemBuffer))]
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
            
            
            int k = 0;
            Entities.WithAll<Bee>().ForEach((Entity entity,ref IsHoldingResource isHoldingResource) =>
            {
                
                for(int i=0;i<pairBuffer.Length;i++)
                {
                    if (pairBuffer[i].BeeEntity == entity)
                    {
                        
                        if (pairBuffer[i].ResourceEntity == Entity.Null && resourceBuffer.Length > 0&&!isHoldingResource.ReachedHome)
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

            allResourceEntities.Dispose();
        }
    }
}