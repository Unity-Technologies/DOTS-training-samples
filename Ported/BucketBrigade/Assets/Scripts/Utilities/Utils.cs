using System.Collections;
using System.Collections.Generic;
using Authoring;
using NUnit.Framework.Internal;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using static Authoring.ConfigAuthoring;
using static UnityEngine.Rendering.DebugUI.Table;

namespace Utilities
{
    public static class Utils 
    {
        
        static readonly ProfilerMarker findWaterMarker = new ProfilerMarker("Utils.FindWater");
        // static readonly ProfilerMarker findFireMarker = new ProfilerMarker("Utils.FindFire");
        static readonly ProfilerMarker findBucketMarker = new ProfilerMarker("Utils.FindBucket");
        static readonly ProfilerMarker dowseFireMarker = new ProfilerMarker("Utils.DowseFire");
        
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

        public static float3 MoveTowards2(in float3 pos, in float3 dest, float speed, float arriveThreshold)
        {
            float3 currPos = pos;

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
            }

            return currPos;
        }

        public static bool IsClose(in float3 pos, in float3 dest, float arriveThreshold)
        {
            bool xClose = math.distance(pos.x, dest.x) <= arriveThreshold;
            bool zClose = math.distance(pos.z, dest.z) <= arriveThreshold;
            return xClose && zClose;
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
            dowseFireMarker.Begin();
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
            dowseFireMarker.End();
        }

        // Find Bucket
        public static Entity FindBucket(ref SystemState state, in float3 position, ref DynamicBuffer<ConfigAuthoring.BucketNode> bucketBuffer, bool wantsFull = false)
        {
            findBucketMarker.Begin();
            var minDistance = float.PositiveInfinity;
            var closestEntity = Entity.Null;

            foreach (var bucket in bucketBuffer)
            {
                var bucketState = state.EntityManager.GetComponentData<Bucket>(bucket.Value);

                if (bucketState.isActive == true) continue;

                var destination = state.EntityManager.GetComponentData<LocalTransform>(bucket.Value);
                var distance = math.distancesq(position, destination.Position);

                if (distance < minDistance)
                {
                    closestEntity = bucket.Value;
                    minDistance = distance;
                }
            }
            findBucketMarker.End();
            return closestEntity;
        }

        // Find Water
        public static Entity FindWater(in float3 position, ref DynamicBuffer<ConfigAuthoring.WaterNode> waterBuffer)
        {
            findWaterMarker.Begin();
            var minDistance = float.PositiveInfinity;
            var closestEntity = Entity.Null;

            foreach (var water in waterBuffer)
            {
                var distance = math.distancesq(position, water.Position);

                if (distance < minDistance)
                {
                    closestEntity = water.Node;
                    minDistance = distance;
                }
            }
            findWaterMarker.End();
            return closestEntity;
        }

        public static int FindWaterIndex(in float3 position, ref NativeArray<ConfigAuthoring.WaterNode> waterBuffer)
        {
            var minDistance = float.PositiveInfinity;
            var closestIndex = -1;

            int index = 0;
            foreach (var water in waterBuffer)
            {
                var distance = math.distancesq(position, water.Position);

                if (distance < minDistance)
                {
                    closestIndex = index;
                    minDistance = distance;
                }

                ++index;
            }

            return closestIndex;
        }

        public static (int, float3) FindFireIndex(in float3 position, in NativeArray<ConfigAuthoring.FlameHeat> fireBuffer, in float flashPoint, float cellSize, int numCols, int numRows)
        {
            var minDistance = float.PositiveInfinity;
            var closestIndex = -1;
            float3 closestPos = new();

            for (int index = 0; index < fireBuffer.Length; ++index)
            {
                if (fireBuffer[index].Value < flashPoint) continue;

                int cellRowIndex = (int)math.floor((float)index / numCols);
                int cellColumnIndex = index % numCols;

                var dest = new float3(cellRowIndex * cellSize, position.y, cellColumnIndex * cellSize);
                
                var distance = math.distancesq(position, dest);
                if (distance < minDistance)
                {
                    closestIndex = index;
                    minDistance = distance;
                    closestPos = dest;
                }
                ++index;
            }

            return (closestIndex, closestPos);
        }
    }
}
