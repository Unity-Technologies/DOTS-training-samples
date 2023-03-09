using System.Collections;
using System.Collections.Generic;
using Authoring;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
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
       
        //Update Filling Bucket
        public static void UpdateFillBucket(ref SystemState state, ref Entity targetBucket, ref LocalTransform botTransform, float waterVolume, float bucketCapacity, float bucketSizeEmpty, float bucketSizeFull, float4 bucketEmptyColor, float4 bucketFullColor)
        {
            float fillFactor = waterVolume / bucketCapacity;
                            
            //paint bucket
            float4 bucketColor = math.lerp(bucketEmptyColor, bucketFullColor, fillFactor);
            state.EntityManager.SetComponentData(targetBucket, new URPMaterialPropertyBaseColor() { Value = bucketColor });
                            
            //scale and move bucket
            LocalTransform bucketTransform = state.EntityManager.GetComponentData<LocalTransform>(targetBucket);
            bucketTransform.Scale = math.lerp(bucketSizeEmpty, bucketSizeFull, fillFactor);
            bucketTransform.Position = botTransform.Position;
            bucketTransform.Position.y += 0.5f;
            state.EntityManager.SetComponentData(targetBucket, bucketTransform);
        }

        //Update Carried Bucket
        public static void UpdateCarriedBucket(ref SystemState state, ref Entity targetBucket, ref LocalTransform botTransform)
        {
            //move bucket
            LocalTransform bucketTransform = state.EntityManager.GetComponentData<LocalTransform>(targetBucket);
            bucketTransform.Position = botTransform.Position;
            bucketTransform.Position.y += 0.5f;
            state.EntityManager.SetComponentData(targetBucket, bucketTransform);
        }
        
        //Update Empty Bucket
        public static void UpdateEmptyBucket(ref SystemState state, ref Entity targetBucket, float bucketSizeEmpty, float4 bucketEmptyColor)
        {
            //paint bucket
            float4 bucketColor = bucketEmptyColor;
            state.EntityManager.SetComponentData(targetBucket, new URPMaterialPropertyBaseColor() { Value = bucketColor });
                            
            //scale and move bucket
            LocalTransform bucketTransform = state.EntityManager.GetComponentData<LocalTransform>(targetBucket);
            bucketTransform.Scale = bucketSizeEmpty;
            bucketTransform.Position.y -= 0.5f;
            state.EntityManager.SetComponentData(targetBucket, bucketTransform);
        }
        
        //Dowse Flame Cell
        public static void DowseFlameCell(ref DynamicBuffer<ConfigAuthoring.FlameHeat> heatMap,int heatMapIndex, int numRows, int numColumns, float coolingStrength, float coolingStrengthFalloff, int splashRadius, float bucketCapacity)
        {
            int targetRow = Mathf.FloorToInt(heatMapIndex / numColumns);
            int targetColumn = heatMapIndex % numColumns;
            heatMap[heatMapIndex] = new ConfigAuthoring.FlameHeat { Value = heatMap[heatMapIndex].Value - coolingStrength };
                            
            for (int rowIndex = -splashRadius; rowIndex <= splashRadius; rowIndex++)
            {
                int currentRow = targetRow - rowIndex;
                if (currentRow >= 0 && currentRow < numRows)
                {
                    for (int columnIndex = -splashRadius; columnIndex <= splashRadius; columnIndex++)
                    {
                        int currentColumn = targetColumn + columnIndex;
                        if (currentColumn >= 0 && currentColumn < numColumns)
                        {
                            float dowseCellStrength = 1f / (Mathf.Abs(rowIndex * coolingStrengthFalloff) + Mathf.Abs(columnIndex * coolingStrengthFalloff));
                            int neighbourIndex = (currentRow * numColumns) + currentColumn;
                            heatMap[neighbourIndex] = new ConfigAuthoring.FlameHeat { Value = heatMap[neighbourIndex].Value - (coolingStrength * dowseCellStrength) * bucketCapacity };
                        }
                    }
                }
            }
        }
    }
}
