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
    ComponentDataFromEntity<RockConfig> m_RockConfigFromEntity;

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
        var rockEntitiesArray = rockpositionQuery.ToEntityArray(Allocator.TempJob);
        //var rockStateArray = rockpositionQuery.ToComponentDataArray<RockConfig>(Allocator.TempJob);

        FarmerTargetSetterJob TargetJob = new FarmerTargetSetterJob
        {
            rockPositionArray = rockPositionArray,
            RockConfigFromEntity = m_RockConfigFromEntity,
            RockEntitiesArray = rockEntitiesArray,
            ECB = ecb
            //rockStateArray = rockStateArray
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

        //[ReadOnly] public NativeArray<Entity> rockEntityArray;

        public void Execute(TransformAspect farmersPosition, ref TargetPosition target, ref Distruction distruction, ref FarmerState state)
        {
            Entity targetEntity = Entity.Null;
            float shortestDistance=999f;
            float3 targetPos= new float3(5,0,5);

           
                for (int i = 0; i < rockPositionArray.Length; i++)
                {
                    float distance = math.distance(farmersPosition.Position, rockPositionArray[i].Position);
                    {
                        if (distance < shortestDistance)
                        {
                            if (RockConfigFromEntity[RockEntitiesArray[i]].state == RockState.isUntargeted)
                            {
                                shortestDistance = distance;
                                targetPos = rockPositionArray[i].Position;
                                targetEntity = RockEntitiesArray[i];
                            }
                        }
                    }
                }
                Debug.Log(targetEntity.ToString());
                if (!state.isWorking)
                    if (targetEntity != Entity.Null) 
                    ECB.SetComponent(targetEntity, new RockConfig { state = RockState.isTargeted });
                target.Target = new float3(targetPos.x, 1, targetPos.z);
                state.isWorking = true;

            if (targetEntity == Entity.Null)
            {
                state.isWorking = false;


            }

            //Debug.Log(math.distance(farmersPosition.Position, target.Target));
            if (math.distance(farmersPosition.Position, target.Target) < 1)
                {
                    distruction.Target = targetEntity;
                }
            
        }
    }
}


