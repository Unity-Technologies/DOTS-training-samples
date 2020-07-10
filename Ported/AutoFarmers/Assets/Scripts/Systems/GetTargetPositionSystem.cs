using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    [UpdateBefore(typeof(VelocitySystem))]
    public class GetTargetPositionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .ForEach((Entity entity, ref TargetPosition targetPosition, in Target target) =>
                {
                    targetPosition.Value = GetComponent<Translation>(target.Value).Value;
                }).ScheduleParallel();
        }
    }
}