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
            //currPos = math.lerp(currPos, dest, speed);
            //float dist = math.distancesq(currPos, dest);
            //if (dist < arriveThreshold)
            //{
            //    return true;
            //}
            //else
            //{
            //    srcTransform.Position = currPos;
            //    return false;
            //}

            bool arrivedX = false;
            bool arrivedZ = false;

            if (currPos.x < dest.x - arriveThreshold)
            {
                currPos.x += speed;
            }
            else if (currPos.x > dest.x + arriveThreshold)
            {
                currPos.x -= speed;
            }
            else
            {
                arrivedX = true;
            }

            // Z POSITION
            if (currPos.z < dest.z - arriveThreshold)
            {
                currPos.z += speed;
            }
            else if (currPos.z > dest.z + arriveThreshold)
            {
                currPos.z -= speed;
            }
            else
            {
                arrivedZ = true;
            }

            if (arrivedX && arrivedZ)
            {
                return true;
            }
            else
            {
                srcTransform.Position = currPos;
                return false;
            }
        }
    }
}
