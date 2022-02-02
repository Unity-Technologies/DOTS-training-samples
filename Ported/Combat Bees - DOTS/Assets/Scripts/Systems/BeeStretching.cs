using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BeeStretching : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<SingletonMainScene>();
    }
    
    protected override void OnUpdate()
    {
        BeeStretchingConstants c = GetSingleton<BeeStretchingConstants>();

        float deltaTime = World.Time.DeltaTime;

        Entities.WithAll<BeeTag>().ForEach(
            (ref NonUniformScale nonUniformScale, in BeeDead beeDead, in Velocity velocity) =>
            {
                if (!beeDead.Value)
                {
                    float magnitude = math.sqrt(velocity.Value.x * velocity.Value.x +
                                                velocity.Value.y * velocity.Value.y + velocity.Value.z +
                                                velocity.Value.z);
                    
                    float stretch = math.clamp(magnitude * c.SpeedStretchSensitivity, c.MinStretch, c.MaxStretch);
                    
                    nonUniformScale.Value.z = stretch;
                    nonUniformScale.Value.x = c.MinStretch - (stretch - c.MinStretch) * c.XStretchMultiplier;
                    nonUniformScale.Value.y = c.MinStretch - (stretch - c.MinStretch) * c.YStretchMultiplier;
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