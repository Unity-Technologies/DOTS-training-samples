using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class HealthScalingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
            .ForEach((Entity healthyObject, ref NonUniformScale scale, ref Translation position, in Health health) =>
            {
                float3 newScale = scale.Value;
                float3 newPosition = position.Value;
            // This just makes sure it works when the pivot is in the centre of the object
            newPosition.y -= scale.Value.y * 0.5f;
                newScale.y = health.Value;
                newPosition.y += newScale.y * 0.5f;
                scale.Value = newScale;
                position.Value = newPosition;
            }).Schedule();
        }
    }
}