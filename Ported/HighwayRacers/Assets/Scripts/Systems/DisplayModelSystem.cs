using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
[UpdateInGroup(typeof(TransformSystemGroup))]
public class DisplayModelSystem : SystemBase
{
    protected override void OnUpdate()
    {
        TrackProperties trackProperties = GetSingleton<TrackProperties>();

        Entities.ForEach((ref LocalToWorld localToWorld, 
            in TrackPosition trackPosition) =>
        {
            float2 carPosition = trackProperties.TrackStartingPoint + 
                new float2(trackPosition.TrackProgress, trackPosition.Lane * (trackProperties.LaneWidth + trackProperties.SeparationWidth));

            var trans = float4x4.Translate(new float3(carPosition.x, carPosition.y, 38));
            localToWorld.Value = trans;

        }).ScheduleParallel();
    }
}