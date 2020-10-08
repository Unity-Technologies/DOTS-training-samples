using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditorInternal;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class AssignTaskSystem : SystemBase
{
    Random m_Random;
    EntityCommandBufferSystem m_ECBSystem;

    EntityQuery pickUpFarmersQuery;
    
    protected override void OnCreate()
    {
        m_Random = new Random(666);
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();

        pickUpFarmersQuery = GetEntityQuery(typeof(PickUpCropTask), ComponentType.Exclude(typeof(TargetEntity)));
    }

    protected override void OnUpdate()
    {
        var ecb = m_ECBSystem.CreateCommandBuffer().AsParallelWriter();
        
        Entities.
            WithName("assign_task").
            WithAll<Farmer>().
            WithNone<DropOffCropTask>().
            WithNone<PickUpCropTask>().
            ForEach(
            (Entity entity, int entityInQueryIndex) =>
            {
                // float rand = m_Random.NextFloat(0, 2);
                // int taskID = (int)math.floor(rand);
                //
                // switch(taskID)
                // {
                //     case 1: //PickUpCrop Task
                //         ecb.AddComponent<PickUpCropTask>(entityInQueryIndex, entity);
                //         break;
                //     case 0: //Idle for one more frame Do Nothing
                //     default:
                //         break;
                // }

                ecb.AddComponent<PickUpCropTask>(entityInQueryIndex, entity);
                
            }).ScheduleParallel();

        m_ECBSystem.AddJobHandleForProducer(Dependency);
        
        NativeArray<Entity> pickUpFarmers = pickUpFarmersQuery.ToEntityArray(Allocator.TempJob);
        NativeArray<TargetEntity> nearestTargetEntity = new NativeArray<TargetEntity>(1, Allocator.TempJob);
        NativeArray<float> nearestCropDist = new NativeArray<float>(1, Allocator.TempJob);
        
        for(int i = 0; i < pickUpFarmers.Length; i++)
        {
            Entity farmerEntity = pickUpFarmers[i];
            float2 farmerPosition = GetComponent<Position>(farmerEntity).Value;
            
            nearestCropDist[0] = float.MaxValue;

            Entities.WithName("assign_pickup_task").WithAll<Crop>().
                //WithNone<Locked>().
                ForEach(
                    (Entity entity, int entityInQueryIndex, in Translation translation) =>
                    {
                        float2 translation2D = new float2(translation.Value.x, translation.Value.z);
                        float distsq = math.distancesq(farmerPosition, translation2D);
                        if(distsq < nearestCropDist[0])
                        {
                            //Need to insert mutex
                            nearestCropDist[0] = distsq;
                            nearestTargetEntity[0] = new TargetEntity{target = entity, targetPosition = translation2D};
                            Debug.Log("New best crop found = "+distsq);
                        }
                    }).Run();

            Debug.Log("BEST CROP = "+nearestCropDist[0]);
            if(nearestCropDist[0] < float.MaxValue)
                EntityManager.AddComponentData(farmerEntity, nearestTargetEntity[0]);
            else
                EntityManager.RemoveComponent<PickUpCropTask>(farmerEntity);
        }
        
        nearestCropDist.Dispose();
        nearestTargetEntity.Dispose();
        pickUpFarmers.Dispose();
   
        
    }

}
