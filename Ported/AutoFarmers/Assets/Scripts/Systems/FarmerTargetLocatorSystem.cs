using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct FarmerTargetLocatorSystem : ISystem
{
    EntityQuery rockpositionQuery;
<<<<<<< HEAD
    ComponentDataFromEntity<RockConfig> m_RockConfigFromEntity;

=======
    
>>>>>>> dots-training-2022-07-eu-group2
    public void OnCreate(ref SystemState state)
    {
        rockpositionQuery = state.GetEntityQuery(typeof(LocalToWorld), typeof(RockTag), typeof(RockConfig));
        m_RockConfigFromEntity = state.GetComponentDataFromEntity<RockConfig>(true);
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        m_RockConfigFromEntity.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var rockPositionArray = rockpositionQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
<<<<<<< HEAD
        var rockEntitiesArray = rockpositionQuery.ToEntityArray(Allocator.TempJob);
        //var rockStateArray = rockpositionQuery.ToComponentDataArray<RockConfig>(Allocator.TempJob);
=======
        var rockEntityArray = rockpositionQuery.ToEntityArray(Allocator.TempJob);
>>>>>>> dots-training-2022-07-eu-group2

        FarmerTargetSetterJob TargetJob = new FarmerTargetSetterJob
        {
            rockPositionArray = rockPositionArray,
<<<<<<< HEAD
            RockConfigFromEntity = m_RockConfigFromEntity,
            RockEntitiesArray = rockEntitiesArray,
            ECB = ecb
            //rockStateArray = rockStateArray
=======
            rockEntityArray = rockEntityArray
>>>>>>> dots-training-2022-07-eu-group2
        };

        TargetJob.Schedule();
    }
    
    [BurstCompile]
    public partial struct FarmerTargetSetterJob: IJobEntity
    {
        [ReadOnly] public NativeArray<Entity> RockEntitiesArray;
        [ReadOnly]
        public NativeArray<LocalToWorld> rockPositionArray;
        [ReadOnly] public ComponentDataFromEntity<RockConfig> RockConfigFromEntity;
        public EntityCommandBuffer ECB;
        //public NativeArray<RockConfig> rockStateArray;

        [ReadOnly] public NativeArray<Entity> rockEntityArray;

        public void Execute(TransformAspect farmersPosition, ref TargetPosition target, ref Distruction distruction)
        {
            var targetEntity = Entity.Null;
            float shortestDistance=999f;
            float3 targetPos= new float3(5,0,5);

            for (int i = 0; i < rockPositionArray.Length; i++)
            {
                float distance = math.distance(farmersPosition.Position, rockPositionArray[i].Position);
                {
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        targetPos = rockPositionArray[i].Position;
<<<<<<< HEAD

                        if (RockConfigFromEntity[RockEntitiesArray[i]].state == RockState.isUntargeted) 
                        {
                            ECB.SetComponent(RockEntitiesArray[i], new RockConfig { state = RockState.isTargeted });
                            //var rockCopy = rockStateArray[i];
                            //rockCopy.state = RockState.isTargeted;
                            //rockStateArray[i] = rockCopy;
                            Debug.Log("state changed");
                            targetEntity = rockEntityArray[i];
                        }
=======
                        targetEntity = rockEntityArray[i];
>>>>>>> dots-training-2022-07-eu-group2
                    }
                }
            }
            target.Target = new float3(targetPos.x,1,targetPos.z);
            Debug.Log(math.distance(farmersPosition.Position, target.Target));
            if (math.distance(farmersPosition.Position, target.Target) < 1)
            {
                distruction.Target = targetEntity;
            }
        }
    }
}


