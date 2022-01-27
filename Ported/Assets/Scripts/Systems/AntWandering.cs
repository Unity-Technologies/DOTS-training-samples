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
    private float m_AntWanderDistance = 1.0f;
    private float m_AntWanderRadius = 0.25f;

    protected override void OnStartRunning()
    {
        var configurationEntity = GetSingletonEntity<Configuration>();
        var config = GetComponent<Configuration>(configurationEntity);
        m_AntWanderDistance = config.AntWanderDistance;
        m_AntWanderRadius = config.AntWanderRadius;
    }

    protected override void OnUpdate()
    {
        float antWanderDistance = m_AntWanderDistance; 
        float antWanderRadius = m_AntWanderRadius; 
        var random = new Random(1234);
        Entities
            .ForEach((ref WanderingSteering wanderingSteering, in Velocity velocity) =>
            {
                float2 wanderDirection = math.normalize(velocity.Direction * antWanderDistance +
                                                    random.NextFloat2Direction() * antWanderRadius);
                wanderingSteering.Value = wanderDirection;
            }).ScheduleParallel();
    }
}