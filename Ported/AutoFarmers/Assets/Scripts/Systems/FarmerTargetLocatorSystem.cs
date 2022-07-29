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
        rockpositionQuery = state.GetEntityQuery(typeof(LocalToWorld), typeof(RockTag), typeof(RockConfig));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var rockPositionArray = rockpositionQuery.ToComponentDataArray<LocalToWorld>(Allocator.TempJob);
        //var rockStateArray = rockpositionQuery.ToComponentDataArray<RockConfig>(Allocator.TempJob);

        FarmerTargetSetterJob TargetJob = new FarmerTargetSetterJob
        {
            rockPositionArray = rockPositionArray,
            //rockStateArray = rockStateArray
        };

        TargetJob.ScheduleParallel();
    }
    
    [BurstCompile]
    public partial struct FarmerTargetSetterJob: IJobEntity
    {
        [ReadOnly]
        public NativeArray<LocalToWorld> rockPositionArray;
        //public NativeArray<RockConfig> rockStateArray;

        public void Execute(TransformAspect farmersPosition, ref TargetPosition target)
        {
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
                        
                        //var rockConfig = rockStateArray[i];
                        //rockConfig.state = RockState.isTargeted;

                        //rockStateArray[i] = rockConfig;
                    }
                }
            }
            target.Target = targetPos;
        }

    }
}
