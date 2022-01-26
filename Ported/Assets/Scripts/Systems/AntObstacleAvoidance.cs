using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/**
 * This system will do an obstacle avoidance Pass on top of Ant steering
 */
[UpdateAfter(typeof(SteeringSystem))]
public partial class AntObstacleAvoidance : SystemBase
{
    private float m_SqrObstacleRadius = 4.0f;
    private EntityQuery m_ObstacleQuery;
    
    protected override void OnStartRunning()
    {
        m_ObstacleQuery = GetEntityQuery(ComponentType.ReadOnly<ObstacleTag>());
    }

    protected override void OnUpdate()
    {
        // First gather all active food
        NativeArray<Translation> obstacleTranslation = m_ObstacleQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        
        // We may sort the array of obstacles
        Entities
            .ForEach((Entity entity, ref ObstacleAvoidanceSteering avoidanceSteering, in Translation antTranslation) =>
            {
                for (int i = 0; i < obstacleTranslation.Length; ++i)
                {
                    float2 obstacleOffset = obstacleTranslation[i].Value.xy - antTranslation.Value.xy;

                    if (math.lengthsq(obstacleOffset) < m_SqrObstacleRadius)
                    {
                        avoidanceSteering.Value = math.normalize(antTranslation.Value.xy - obstacleTranslation[i].Value.xy);
                        break;
                    }
                }
            }).WithoutBurst().Run();
        
        
        obstacleTranslation.Dispose();
    }
}