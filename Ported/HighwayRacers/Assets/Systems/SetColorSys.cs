using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace HighwayRacer
{
    public class SetColorSys : SystemBase
    {
        
        public readonly static float3 cruiseColor = new float3(0.5f, 0.5f, 0.5f);
        public readonly static float3 fastestColor = new float3(0, 1.0f, 0);
        public readonly static float3 slowestColor = new float3(1.0f, 0.0f, 0);
        
        public readonly static float4 other = new float4(1, 0, 0, 1);
        
        protected override void OnUpdate()
        {
            const float minSpeed = 10.0f;

            Entities.ForEach((ref Color color, in Speed speed, in DesiredSpeed desiredSpeed) =>
            {
                if (speed.Val >= desiredSpeed.Unblocked)
                {
                    var percentage = (speed.Val - desiredSpeed.Unblocked) / (desiredSpeed.Overtake - desiredSpeed.Unblocked);
                    color.Val = new float4(math.lerp(cruiseColor, fastestColor, percentage), 1.0f);
                }
                else
                {
                    var percentage = (desiredSpeed.Unblocked - speed.Val) / (desiredSpeed.Unblocked - minSpeed);
                    color.Val = new float4(math.lerp(cruiseColor, slowestColor, percentage), 1.0f);
                }
            }).ScheduleParallel();

            // for debug, make left blue, right yellow
            // Entities.ForEach((ref Color color, in MergingLeft mergingLeft) =>
            // {
            //     color.Val = new float4(0, 0.2f, 0.8f, 1.0f);
            // }).Run();
            //
            // Entities.ForEach((ref Color color, in MergingRight mergingRight) =>
            // {
            //     color.Val = new float4(1.0f, 1.0f, 0, 1.0f);
            // }).Run();
        }
    }
}




