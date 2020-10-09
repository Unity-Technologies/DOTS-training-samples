using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(DeduplicationSystem))]
public class PickUpCropSystem : SystemBase
{
    EntityCommandBufferSystem m_ECBSystem;
    EntityQuery depotsQuery;
    
    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        depotsQuery = GetEntityQuery(typeof(Depot));
    }
    
    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        const float reachDistance = 0.1f;
        
        NativeArray<Entity> depotsEntity = depotsQuery.ToEntityArray(Allocator.TempJob);
        
        Entities
            .WithName("pickup_system_farmers")
            .WithReadOnly(depotsEntity)
            .WithDisposeOnCompletion(depotsEntity)
            .WithAll<Farmer>()
            .ForEach((
                Entity entity, 
                int entityInQueryIndex,
                ref PickUpCropTask task, 
                ref TargetEntity target, 
                in Position position) =>
            {
                float distanceToTarget = math.distance(position.Value, target.targetPosition);
                if(distanceToTarget < reachDistance)
                {
                    Entity bestDepot = depotsEntity[0];
                    float minDistToDepot = float.MaxValue;
                
                    for(int depotIndex = 0; depotIndex< depotsEntity.Length; depotIndex++)
                    {
                        Translation translation = GetComponent<Translation>(depotsEntity[depotIndex]);
                        float2 depotPosition = new float2(translation.Value.x,translation.Value.z);
                        float dist = math.distancesq(position.Value, depotPosition);
                        if(dist < minDistToDepot)
                        {
                            minDistToDepot = dist;
                            bestDepot = depotsEntity[depotIndex];
                        }
                    }
                
                    //Updating farmer task
                    ecb.RemoveComponent<PickUpCropTask>(entityInQueryIndex, entity);
                    ecb.AddComponent<DropOffCropTask>(entityInQueryIndex, entity);
                    ecb.SetComponent(entityInQueryIndex, entity, new TargetEntity(){target = bestDepot, targetPosition = GetComponent<Position>(bestDepot).Value});
                    
                    //Updating carried crop
                    Entity cropPlainsEntity = target.target;
                    Entity cropEntity = GetComponent<CropReference>(cropPlainsEntity).crop;
                    ecb.AddComponent(entityInQueryIndex, cropEntity, new CropCarried{FarmerOwner = entity});
                    ecb.AddComponent(entityInQueryIndex, cropEntity, new TargetEntity(){target = bestDepot, targetPosition = GetComponent<Position>(bestDepot).Value});

                    ecb.RemoveComponent<CropReference>(entityInQueryIndex, cropPlainsEntity);
                    ecb.RemoveComponent<Assigned>(entityInQueryIndex, cropPlainsEntity);
                }
                
            }).ScheduleParallel();
        
        m_ECBSystem.AddJobHandleForProducer(Dependency);
    }
}
