using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace CombatBees.Testing.BeeFlight
{
    [UpdateAfter(typeof(BeeMovementSystemBuffer))]
    public partial class ResourceMovementSystemBuffer : SystemBase
    {
        private int k = 0;
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SingeltonBeeMovement>();
            RequireSingletonForUpdate<BufferSingelton>();
        }
        
        protected override void OnUpdate()
        {
            var allTranslations = GetComponentDataFromEntity<Translation>(true);
            var pairBuffer = GetBuffer<BeeResourcePair>(GetSingletonEntity<BufferSingelton>());
            var heldBuffer = GetBuffer<HeldResourceBuffer>(GetSingletonEntity<SecondSingeltonBuffer>());
            //buffer.Clear();
            Entities.WithAll<Bee>().ForEach((Entity entity, int entityInQueryIndex,ref IsHoldingResource isHoldingResource, in BeeTargets beeTargets) =>
            {
                // Entity beeResourceTarget = beeTargets.ResourceTarget;
                //
                // if (isHoldingResource.PickedUp)
                // {  
                //     heldBuffer.Add(new HeldResourceBuffer
                //     {
                //         Resource = beeResourceTarget,
                //         Bee = entity,
                //     });
                //     isHoldingResource.PickedUp = false;
                // }
                //
                // if (!isHoldingResource.Value)
                // {
                //     for (int i=0;i<heldBuffer.Length;i++)
                //     {
                //         if (heldBuffer[i].Bee == entity)
                //         {
                //             heldBuffer.RemoveAt(i);
                //         }
                //     }
                // }
                
                foreach (var pair in pairBuffer)
                {
                    if (isHoldingResource.JustPickedUp&& !isHoldingResource.ReachedHome)
                    {
                        if (pair.BeeEntity == entity && pair.ResourceEntity != Entity.Null)
                        {

                            heldBuffer.Add(new HeldResourceBuffer
                            {
                                Resource = pair.ResourceEntity,
                                Bee = pair.BeeEntity
                            });
                            isHoldingResource.JustPickedUp = false;

                        }
                        
                    }
                    //the resource is dropped so we are removing it from the held buffer 
                    if (!isHoldingResource.Value&&isHoldingResource.ReachedHome)
                    {
                        for (int i = 0; i < heldBuffer.Length; i++)
                        {
                            if (heldBuffer[i].Bee == entity)
                            {
                                heldBuffer.RemoveAt(i);
                            }
                        }

                        isHoldingResource.ReachedHome = false;
                    }
                }
            }).Schedule();
            Entities.WithAll<Resource>().ForEach(
                (Entity entity, ref Translation translation, ref Holder holder) =>
                {
                    bool inBuffer = false;
                    foreach (var pair in heldBuffer)
                    {
                        if (entity == pair.Resource)
                        {
                             inBuffer = true;
                             translation.Value = allTranslations[pair.Bee].Value;
                            
                        }
                    }
                    if(!inBuffer)
                    {
                        if (translation.Value.y > 0.5f)
                        {
                            translation.Value.y -= 3.0f * World.Time.DeltaTime;
                        }
                        else if (translation.Value.y < 0.5f)
                        {
                            translation.Value.y = 0.5f;
                        }
                      
                    }
                }).WithoutBurst().Run();

        }
    }
}