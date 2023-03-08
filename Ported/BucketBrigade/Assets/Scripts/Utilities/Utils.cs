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
        public static bool MoveTowards(ref LocalTransform srcTransform, float3 dest, float speed, float arriveThreshold)
        {
            float3 currPos = srcTransform.Position;
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
                srcTransform.Position = currPos;
                return false;
            }
        }
    }
}
