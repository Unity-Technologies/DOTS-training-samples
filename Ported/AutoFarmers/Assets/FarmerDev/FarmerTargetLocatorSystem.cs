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
        rockpositionQuery = state.GetEntityQuery(typeof(LocalToWorld), typeof(Rock));
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var test = rockpositionQuery.ToComponentDataArray<LocalToWorld>(Allocator.Temp);
        // float3[] RockPositionArray= new float3[999];
        // int iterator=0;
        // foreach (var transform in SystemAPI.Query<TransformAspect>().WithAll<Rock>())
        // {
        //     RockPositionArray[iterator] = transform.Position;
        //    iterator++;
        // }
            
        FarmerTargetSetterJob TargetJob = new FarmerTargetSetterJob
        {
            rockPositionArray = test
        };

        TargetJob.ScheduleParallel();
    }
    
    [BurstCompile]
    public partial struct FarmerTargetSetterJob: IJobEntity
    {
       public NativeArray<LocalToWorld> rockPositionArray;
      

        public void Execute(TransformAspect farmersPosition, ref TargetPosition target)
        {
            float shortestDistance=999f;
            float3 targetPos= new float3(5,0,5);

            for (int i = 0; i < rockPositionArray.Length; i++)
            {
                float distance = math.distance(farmersPosition.Position, rockPositionArray[i].Position);
                if (distance<shortestDistance)
                {
                    shortestDistance = distance;
                    targetPos = rockPositionArray[i].Position;
                }
            }
            
            target.Target = targetPos;
        }

    }
}
