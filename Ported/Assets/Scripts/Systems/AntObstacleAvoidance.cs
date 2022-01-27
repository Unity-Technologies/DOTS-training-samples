using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/**
 * This system will do an obstacle avoidance Pass on top of Ant steering
 */
public partial class AntObstacleAvoidance : SystemBase
{
    private float m_SqrObstacleRadius = 4.0f;
    private float m_AntObstacleAvoidanceDistance = 0.5f;
    private EntityQuery m_ObstacleQuery;
    
    protected override void OnStartRunning()
    {
        var configurationEntity = GetSingletonEntity<Configuration>();
        var config = GetComponent<Configuration>(configurationEntity);
        m_SqrObstacleRadius = config.ObstacleRadius * config.ObstacleRadius;
        m_AntObstacleAvoidanceDistance = config.AntObstacleAvoidanceDistance;
        m_ObstacleQuery = GetEntityQuery(ComponentType.ReadOnly<ObstacleTag>(), ComponentType.ReadOnly<Translation>());
    }

    protected override void OnUpdate()
    {
        // Forces a dependency Complete before we access ToComponentDataArray<Translation> below. Bug or regression in ECS?
        Entities.ForEach((in Translation Translation) => { }).Run();
        
        // First gather all active food
        NativeArray<Translation> obstacleTranslation = m_ObstacleQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        float sqrObstacleRadius = m_SqrObstacleRadius;
        float antObstacleAvoidanceDistance = m_AntObstacleAvoidanceDistance;
        
        // We may sort the array of obstacles
        Entities
            .ForEach((Entity entity, ref ObstacleAvoidanceSteering avoidanceSteering, in Translation antTranslation, in Velocity antVelocity) =>
            {
                for (int i = 0; i < obstacleTranslation.Length; ++i)
                {
                    float2 predictedPosition = antTranslation.Value.xy + antVelocity.Direction * antObstacleAvoidanceDistance;
                    float2 obstacleOffset = obstacleTranslation[i].Value.xy - predictedPosition;
                    if (math.lengthsq(obstacleOffset) < sqrObstacleRadius)
                    {
                        avoidanceSteering.Value = math.normalize(predictedPosition - obstacleTranslation[i].Value.xy);
                        break;
                    }
                }
            }).WithDisposeOnCompletion(obstacleTranslation).Schedule();
    }
}