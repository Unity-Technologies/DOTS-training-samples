using Unity.Entities;
using Unity.Mathematics;

public class MoveTowardsTargetSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        Entities
            .WithName("MoveTowardsTarget")
            .ForEach((ref Pos pos, ref Target target,
                in Speed speed) =>
            {
                float speedThisFrame = speed.Value * deltaTime;
                float2 offset = target.Position - pos.Value;
                target.ReachedTarget = math.lengthsq(offset) <= speedThisFrame * speedThisFrame;
                
                if (target.ReachedTarget)
                {
                    pos.Value = target.Position;
                }
                else
                {
                    pos.Value += math.normalize(offset) * speedThisFrame;
                }
            }).ScheduleParallel();
    }
}