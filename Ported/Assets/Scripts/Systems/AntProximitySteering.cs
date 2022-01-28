using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/**
 * This system compute the food final approach steering behavior : Ants will seek food source when they are in food range, with clear line of sight.
 */
[UpdateBefore(typeof(SteeringSystem))]
public partial class AntProximitySteering : SystemBase
{
    private EntityQuery m_FoodQuery;
    private float2 m_NestPosition;
    private float m_FoodNestRadius = 4.0f;
    private float m_AntVisibilityDistance = 0.25f;
    
    protected override void OnStartRunning()
    {
        m_FoodQuery = GetEntityQuery(ComponentType.ReadOnly<ResourceTag>(), ComponentType.ReadOnly<Translation>());
        var colonyEntity = GetSingletonEntity<ColonyTag>();
        var nestTranslation = GetComponent<Translation>(colonyEntity);
        var scale = GetComponent<NonUniformScale>(colonyEntity);
        m_NestPosition = nestTranslation.Value.xy;
        m_FoodNestRadius = scale.Value.x;
    }

    protected override void OnUpdate()
    {
        // First gather all active food
        NativeArray<Translation> foodTranslation = m_FoodQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var grid = GetSingleton<Grid2D>();
        var obstacleBuffer = GetBuffer<ObstaclePositionAndRadius>(GetSingletonEntity<Grid2D>());
        float2 nestPosition = m_NestPosition;
        float foodNestRadius = m_FoodNestRadius;
        float antVisibilityDistance = m_AntVisibilityDistance;
        
        // We may sort the array of food and optimize the Food looping
        Entities.WithReadOnly(foodTranslation).WithReadOnly(obstacleBuffer)
            .ForEach((ref ProximitySteering proximitySteering, ref Loadout loadout, in Velocity velocity, in Translation antTranslation) =>
            {
                if (loadout.Value > 0)
                {
                    float2 nestOffset = nestPosition - antTranslation.Value.xy;
                    float dist = math.length(nestOffset);

                    if (dist > antVisibilityDistance)
                    {
                        proximitySteering.Value = float2.zero;
                    }
                    else if (Utils.LinecastObstacles(grid, obstacleBuffer, nestPosition, antTranslation.Value.xy) == true)
                    {
                        proximitySteering.Value = float2.zero;
                    }
                    else
                    {
                        proximitySteering.Value = nestOffset / dist;
                    
                        if (dist < foodNestRadius) 
                        {
                            loadout.Value = 0;
                        }
                    }
                }
                else
                {
                    proximitySteering.Value = float2.zero;
                    for (int i = 0; i < foodTranslation.Length; ++i)
                    {
                        float2 foodOffset = foodTranslation[i].Value.xy - antTranslation.Value.xy;
                        float dist = math.length(foodOffset);
                        
                        if (dist > antVisibilityDistance)
                        {
                            continue;
                        }
                        
                        if (Utils.LinecastObstacles(grid, obstacleBuffer, nestPosition, antTranslation.Value.xy) == true)
                        {
                            continue;
                        }

                        proximitySteering.Value = foodOffset / dist;
                        
                        // tHis code handle the loading / unloading
                        if (dist < foodNestRadius) 
                        {
                            loadout.Value = 1;
                        }
                        break;
                    }
                }
            }).WithDisposeOnCompletion(foodTranslation).ScheduleParallel();
    }

    private bool HasLineOfSight(float2 translationA, float2 translationB)
    {
        return false;
    }
}