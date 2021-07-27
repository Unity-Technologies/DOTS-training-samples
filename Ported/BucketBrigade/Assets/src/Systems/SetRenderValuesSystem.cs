using System;
using src.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class SetRenderValuesSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithName("WorkerRenderValues").WithNone<EcsBucket>().WithBurst().ForEach((ref LocalToWorld localToWorld, in Position pos) =>
            {
                //localToWorld.Value.c3 = new float4(pos.Value.x, 0, pos.Value.y, 0);
                localToWorld.Value = float4x4.TRS(new float3(pos.Value.x, 0, pos.Value.y), Quaternion.identity, Vector3.one);
            }).ScheduleParallel();
    
            // Buckets:
            Entities.WithName("BucketsRenderValues").WithBurst().WithNone<FillUpBucketTag>().ForEach((ref LocalToWorld localToWorld, ref URPMaterialPropertyBaseColor color, in Position pos, in EcsBucket bucket) =>
            {
                WriteBucketRendererValues(ref localToWorld, ref color, pos, bucket, Quaternion.identity, new float3(0f, 1f, 0f));
            }).ScheduleParallel();  
            
            var fillingUpRotation = Quaternion.Euler(45f, 0f, 45f);
            Entities.WithName("FillingUpBucketsRenderValues").WithBurst().WithAll<FillUpBucketTag>().ForEach((ref LocalToWorld localToWorld, ref URPMaterialPropertyBaseColor color, in Position pos, in EcsBucket bucket) =>
            {
                WriteBucketRendererValues(ref localToWorld, ref color, pos, bucket, fillingUpRotation, new float3(0.5f, 0, 0.5f));
            }).ScheduleParallel();
        }
        
        static void WriteBucketRendererValues(ref LocalToWorld localToWorld,  ref URPMaterialPropertyBaseColor color, Position pos, EcsBucket bucket, Quaternion rotation, float3 bucketWorkerOffset)
        {
            localToWorld.Value = float4x4.TRS(new float3(pos.Value.x, 0, pos.Value.y) + bucketWorkerOffset, rotation, (Vector3.one * 0.5f));
            var targetColor = Color.Lerp(Color.red, Color.cyan, bucket.WaterLevel);
            color.Value = new float4(targetColor.r, targetColor.g, targetColor.b, 1f);
        }
    }
}
