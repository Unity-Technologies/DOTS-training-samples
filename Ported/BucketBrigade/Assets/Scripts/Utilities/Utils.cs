using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Utilities
{
    public static class Utils 
    {
        public static bool MoveTowards(RefRW<LocalTransform> srcTransform, float3 dest, float speed, float arriveThreshold)
        {
            float3 currPos = srcTransform.ValueRW.Position;
            float movementSpeed = speed;

            currPos = math.lerp(currPos, dest, speed);
            float dist = math.distancesq(currPos, dest);
            if (dist < arriveThreshold)
            {
                Debug.Log("Arrived!");
                return true;
            }
            else
            {
                Debug.Log("Moving!");
                srcTransform.ValueRW.Position = currPos;
                return false;
            }
        }
    }
}
