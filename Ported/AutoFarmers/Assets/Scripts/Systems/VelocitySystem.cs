using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class VelocitySystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float speed = 2f;
            
            Entities
                .ForEach((Entity entity, ref Velocity velocity, in Translation translation, in Target target) =>
                {
                    Translation targetPosition = GetComponent<Translation>(target.Value);
                    float3 delta = targetPosition.Value - translation.Value;
                    float3 direction = math.normalize(delta);
                    float distance = math.length(delta);
                    float finalSpeed = speed * math.clamp(distance, 0f, 1f);
                    float3 targetVelocity = direction * finalSpeed;
                    velocity.Value = math.lerp(velocity.Value, targetVelocity, 0.5f);
                }).ScheduleParallel();
        }
    }
}
