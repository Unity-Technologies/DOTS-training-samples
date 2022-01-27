using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

/**
 * This system compute make ants Wander around
 */
[UpdateBefore(typeof(SteeringSystem))]
public partial class AntWandering : SystemBase
{
    private float m_WanderAmount = 0.25f;

    protected override void OnStartRunning()
    {
        var configurationEntity = GetSingletonEntity<Configuration>();
        var config = GetComponent<Configuration>(configurationEntity);
        m_WanderAmount = config.AntWanderAmount;
    }

    protected override void OnUpdate()
    {
        float wanderAmount = m_WanderAmount;
        var random = new Random(1234);
        Entities
            .ForEach((ref WanderingSteering wanderingSteering, in Velocity velocity) =>
            {
                float2 sideComponent = new float2(-velocity.Direction.y, velocity.Direction.x) *
                                       random.NextFloat(-wanderAmount, wanderAmount);
                wanderingSteering.Value = math.normalize(velocity.Direction + sideComponent);
            }).ScheduleParallel();
    }
}