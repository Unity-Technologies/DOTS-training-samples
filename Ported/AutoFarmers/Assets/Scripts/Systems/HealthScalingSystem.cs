using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class HealthScalingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .ForEach((Entity healthyObject, ref NonUniformScale scale, in Health health) =>
        {
            float3 newScale = scale.Value;
            //newScale.y = health.Value;
            scale.Value = newScale;
        }).Schedule();
    }
}