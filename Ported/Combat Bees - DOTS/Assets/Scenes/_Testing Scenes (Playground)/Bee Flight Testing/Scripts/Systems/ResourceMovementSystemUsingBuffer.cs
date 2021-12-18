using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace CombatBees.Testing.BeeFlight
{
    public partial class ResourceMovementSystemUsingBuffer : SystemBase
    {
        private int k = 0;
        protected override void OnCreate()
        {
           // RequireSingletonForUpdate<SingeltonBeeMovement>();
            RequireSingletonForUpdate<BufferSingelton>();
        }
        
        protected override void OnUpdate()
        {
            var allTranslations = GetComponentDataFromEntity<Translation>(true);
            var buffer = GetBuffer<BeeResourcePair>(GetSingletonEntity<BufferSingelton>());
            //buffer.Clear();
            Entities.WithAll<Bee>().ForEach((Entity entity, int entityInQueryIndex,ref IsHoldingResource isHoldingResource, in BeeTargets beeTargets) =>
            {
                Entity beeResourceTarget = beeTargets.ResourceTarget;
               
                if (isHoldingResource.PickedUp)
                {  
                    buffer.Add(new BeeResourcePair
                    {
                        ResourceEntity = beeResourceTarget,
                        BeeEntity = entity,
                       // index=k
                    });
                   // k++;
                    isHoldingResource.PickedUp = false;
                }

                if (!isHoldingResource.Value)
                {
                    for (int i=0;i<buffer.Length;i++)
                    {
                        if (buffer[i].BeeEntity == entity)
                        {
                            buffer.RemoveAt(i);
                        }
                    }
                }
            }).Schedule();
            Entities.WithAll<Resource>().ForEach(
                (Entity entity, ref Translation translation, ref Holder holder) =>
                {
                    foreach (var pair in buffer)
                    {
                        if (entity == pair.ResourceEntity)
                            {
                                translation.Value = allTranslations[pair.BeeEntity].Value;
                            }
                            else
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
                        
                    }
                }).WithoutBurst().Run();

        }
    }
}