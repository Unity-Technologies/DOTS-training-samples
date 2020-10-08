
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class FarmerMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        
        Entities
            .WithAll<Farmer>()
            .ForEach((ref Position position, in TargetEntity targetEntity, in Speed speed) =>
            {
                float2 targetPos = targetEntity.targetPosition;
                
                float2 direction = math.normalize(targetPos - position.Value);
                position.Value = position.Value + direction * speed.Value * deltaTime;

            }).ScheduleParallel();
    }
}
