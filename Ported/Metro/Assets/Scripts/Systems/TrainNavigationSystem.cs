using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class TrainNavigationSystem : SystemBase
{

    float3 GetPositionOnCurve(float t)
    {
        float3 start = new float3(0, 0, 0);
        float3 destination = new float3(50, 0, 0);
        return math.lerp(start, destination, t);
    }
        
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.WithAll<TrainComponent>()
            .ForEach((ref SpeedComponent speed, ref TrackPositionComponent trackPosition) => {
            trackPosition.Value = math.frac(trackPosition.Value + speed.Value * deltaTime);
        }).Schedule();
    }
}
