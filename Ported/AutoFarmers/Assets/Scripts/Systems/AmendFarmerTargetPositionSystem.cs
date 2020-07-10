using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AutoFarmers
{
    [UpdateAfter(typeof(GetTargetPositionSystem))]
    [UpdateBefore(typeof(VelocitySystem))]
    public class AmendFarmerTargetPositionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<Farmer_Tag>()
                .ForEach((Entity entity, ref TargetPosition targetPosition) =>
                {
                    float3 position = targetPosition.Value;
                    position.y = 0.5f;
                    targetPosition.Value = position;
                }).ScheduleParallel();
        }
    }
}