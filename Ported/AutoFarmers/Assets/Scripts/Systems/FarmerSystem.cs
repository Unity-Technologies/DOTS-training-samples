using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

[BurstCompile]
partial struct FarmerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float moveOffset = 1.5f;

        foreach (var farmer in SystemAPI.Query<FarmerAspect>())
        {
            //Let's find a rock to destroy
            if(farmer.FarmerState == FarmerStates.FARMER_STATE_ROCKDESTROY)
            {
                float closestSqrMag = math.INFINITY;
                RockAspect closestRock = new RockAspect();

                foreach (var rock in SystemAPI.Query<RockAspect>())
                {
                    //Let's find closest rock
                    float3 diff = rock.Transform.WorldPosition - farmer.Transform.WorldPosition;
                    float sqrMag = math.lengthsq(diff);
                    if (sqrMag < closestSqrMag)
                    {
                        closestRock = rock;
                        closestSqrMag = sqrMag;
                    }
                }

                float3 diff2 = farmer.Transform.WorldPosition - closestRock.Transform.WorldPosition;
                farmer.MoveTarget = closestRock.Transform.WorldPosition + moveOffset * math.normalize(diff2);

                if (math.lengthsq(diff2) <= (moveOffset * moveOffset))
                {
                    //Let's hurt the rock.
                    closestRock.Health -= 1;
                }
            }
        }
    }
}
