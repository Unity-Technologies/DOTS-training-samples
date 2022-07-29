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
    
    public void OnCreate(ref SystemState state)
    {
        rockpositionQuery = state.GetEntityQuery(typeof(LocalToWorld), typeof(RockTag));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var rockPositionArray = rockpositionQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
        var rockEntityArray = rockpositionQuery.ToEntityArray(Allocator.TempJob);

        FarmerTargetSetterJob TargetJob = new FarmerTargetSetterJob
        {
            rockPositionArray = rockPositionArray,
            rockEntityArray = rockEntityArray,
        };

        TargetJob.ScheduleParallel();
    }
    
    [BurstCompile]
    public partial struct FarmerTargetSetterJob: IJobEntity
    {
        [ReadOnly]
        public NativeArray<LocalToWorld> rockPositionArray;

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
                    if (distance<shortestDistance)
                    {
                        shortestDistance = distance;
                        targetPos = rockPositionArray[i].Position;
                        targetEntity = rockEntityArray[i];
                    }
                }
            }
            target.Target = new float3(targetPos.x,1,targetPos.z);
            if (math.distance(farmersPosition.Position, target.Target) < 1)
            {
                distruction.Target = targetEntity;
            }
        }

    }
}


