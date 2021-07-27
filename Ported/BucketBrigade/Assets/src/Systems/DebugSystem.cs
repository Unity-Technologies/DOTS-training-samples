using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [AlwaysUpdateSystem]
    public class DebugSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithName("DrawFetchersNoBuckets")
                .WithAll<BucketFetcherWorkerTag>()
                .WithNone<WorkerIsHoldingBucket>()
                .ForEach((in Position pos) =>
            {
                Debug.DrawLine(new Vector3(pos.Value.x, 0.0f, pos.Value.y ),
                    new Vector3(pos.Value.x, 10.0f, pos.Value.y), Color.red);
            }).Run();

            Entities
                .WithName("DrawFetchersWithBuckets")
                .WithAll<BucketFetcherWorkerTag, WorkerIsHoldingBucket>()
                .ForEach((in Position pos) =>
                {
                    Debug.DrawLine(new Vector3(pos.Value.x, 0.0f, pos.Value.y),
                        new Vector3(pos.Value.x, 10.0f, pos.Value.y), Color.yellow);
                }).Run();

            Entities
                .WithoutBurst()
                .WithName("DrawBuckets")
                .WithAll<EcsBucket>()
                .ForEach((in Position pos, in EcsBucket bucket) =>
                {
                    DrawCross(new Vector3(pos.Value.x, 5.0f, pos.Value.y), 1.0f + bucket.WaterLevel * 4.0f, Color.Lerp(Color.white, Color.blue, bucket.WaterLevel));
                }).Run();


        }


        private void DrawCross(Vector3 position, float size, Color color)
        {
            Debug.DrawLine(position - Vector3.up * size, position + Vector3.up * size, color);
            Debug.DrawLine(position - Vector3.right * size, position + Vector3.right * size, color);
            Debug.DrawLine(position - Vector3.forward * size, position + Vector3.forward * size, color);
        }
    }
}
