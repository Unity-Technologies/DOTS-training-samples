using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BeeStretching : SystemBase
{
    protected override void OnUpdate()
    {
        float speedStretch = 0.1f;
        float deltaTime = World.Time.DeltaTime;

        Entities.WithAll<BeeTag>().ForEach(
            (ref NonUniformScale nonUniformScale, in BeeDead beeDead, in Velocity velocity) =>
            {
                if (!beeDead.Value)
                {
                    float magnitude = math.sqrt(velocity.Value.x * velocity.Value.x +
                                                velocity.Value.y * velocity.Value.y + velocity.Value.z +
                                                velocity.Value.z);
                    float stretch = math.max(1f, magnitude * speedStretch);
                    nonUniformScale.Value.z = stretch;
                    nonUniformScale.Value.x = 2f / stretch;
                    nonUniformScale.Value.y = 2f / stretch;
                }
                else if (nonUniformScale.Value.z > 1f)
                {
                    nonUniformScale.Value = new float3(1f, 1f, 1f);
                }
                else
                {
                    if (nonUniformScale.Value.x > 0.01f)
                    {
                        nonUniformScale.Value = nonUniformScale.Value -
                                                new float3(0.1f, 0.1f, 0.1f) *
                                                new float3(deltaTime, deltaTime, deltaTime);
                    }
                }
            }).ScheduleParallel();
    }
}