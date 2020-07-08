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
            float delta = UnityEngine.Time.deltaTime;
            float speed = 1f;
            
            Entities
                .ForEach((Entity entity, ref Velocity velocity, in Translation translation, in Target target) =>
                {
                    Translation targetPosition = GetComponent<Translation>(target.Value);
                    float3 direction = math.normalize(targetPosition.Value - translation.Value);
                    float3 targetVelocity = direction * speed;
                    velocity.Value = math.lerp(velocity.Value, targetVelocity, 0.5f);
                }).ScheduleParallel();
        }

        private float2 GetVelocity()
        {
            throw new System.NotImplementedException();
        }
    }
}
