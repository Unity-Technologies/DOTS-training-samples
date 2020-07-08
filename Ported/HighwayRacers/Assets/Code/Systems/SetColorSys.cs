using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace HighwayRacer
{
    [UpdateAfter(typeof(MergingOvertakeSys))]
    public class SetColorSys : SystemBase
    {
        public readonly static float3 cruiseColor = new float3(0.5f, 0.5f, 0.5f);
        public readonly static float3 fastestColor = new float3(0, 1.0f, 0);
        public readonly static float3 slowestColor = new float3(1.0f, 0.0f, 0);
        
        protected override void OnUpdate()
        {
            const float minSpeed = 10.0f;

            Entities.ForEach((ref URPMaterialPropertyBaseColor color, in Speed speed, in DesiredSpeed desiredSpeed) =>
            {
                if (speed.Val >= desiredSpeed.Unblocked)
                {
                    var percentage = (speed.Val - desiredSpeed.Unblocked) / (desiredSpeed.Overtake - desiredSpeed.Unblocked);
                    color.Value = new float4(math.lerp(cruiseColor, fastestColor, percentage), 1.0f);
                }
                else
                {
                    var percentage = (desiredSpeed.Unblocked - speed.Val) / (desiredSpeed.Unblocked - minSpeed);
                    color.Value = new float4(math.lerp(cruiseColor, slowestColor, percentage), 1.0f);
                }
            }).ScheduleParallel();
        }
    }
}




