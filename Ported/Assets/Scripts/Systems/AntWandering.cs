using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

/**
 * This system compute make ants Wander around
 */
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
        Entities
            .ForEach((Entity entity, ref WanderingSteering wanderingSteering, in Velocity velocity) =>
            {
                wanderingSteering.Value = new float2(-velocity.Direction.y, velocity.Direction.x) * Random.Range(-wanderAmount, wanderAmount);
            }).Schedule();
    }
}