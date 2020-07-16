using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class Jitter : SystemBase
{
    Random m_Random;

    protected override void OnCreate()
    {
        m_Random = new Random(0x8465681);
    }

    protected override void OnUpdate()
    {
        // bee.velocity += Random.insideUnitSphere * (flightJitter * deltaTime);
        // bee.velocity *= (1f-damping);

        var deltaTime = Time.DeltaTime;
        var flightJitter = BeeManager.Instance.flightJitter;
        var damping = BeeManager.Instance.damping;
        var random = m_Random;

        Entities
            .WithAll<Size>()
            .WithNone<Dead>()
            .ForEach((ref Velocity velocity) =>
        {
            velocity.Value += random.NextFloat3Direction() * (flightJitter * deltaTime);
            velocity.Value *= (1f - damping);
        }).ScheduleParallel();

        m_Random = random;
    }
}
