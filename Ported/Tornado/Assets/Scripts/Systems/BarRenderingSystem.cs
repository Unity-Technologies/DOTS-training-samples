using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public partial class BarRenderingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var time = (float)Time.ElapsedTime;

            Entities
                .ForEach((ref LocalToWorld localToWorld, in Components.Bar bar) =>
                {
                    localToWorld.Value = float4x4.TRS(
                        float3.zero,
                        quaternion.identity,
                        new float3(bar.thickness, bar.thickness, math.sin(time) + 1.0f)
                    );
                }).ScheduleParallel();
        }
    }
}