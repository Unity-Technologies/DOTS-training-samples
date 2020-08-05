using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class CarRenderingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithName("CarRenderingSystem")
            .ForEach((
                ref LocalToWorld ltw,
                in Size size,
                in Offset offset,
                in CurrentSegment currentSegment,
                in Progress progress
            ) =>
            {
                //Get segment
                var segment = currentSegment.Value;

                //TRS
                var position = math.lerp(segment.Start,segment.End,progress.Value);
                var rotation = float4x4.LookAt(position,segment.End,new float3(0,1,0));
                var scale = float4x4.Scale(size.Value);

                //Set the localToWorld
                var translation = float4x4.Translate(position);
                ltw.Value = translation * rotation * scale;
               // ltw.Value = math.mul(math.mul(float4x4.Translate(position), rotation) , scale);

            }).ScheduleParallel();
    }
}