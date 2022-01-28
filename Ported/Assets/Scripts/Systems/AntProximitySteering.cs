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
        float2 nestPosition = m_NestPosition;
        float foodNestRadius = m_FoodNestRadius;
        
        // We may sort the array of food and optimize the Food looping
        Entities.WithReadOnly(foodTranslation)
            .ForEach((ref ProximitySteering proximitySteering, ref Loadout loadout, in Translation antTranslation) =>
            {
                if (loadout.Value > 0)
                {
                    float2 nestOffset = nestPosition - antTranslation.Value.xy;
                    float dist = math.length(nestOffset); 
                    
                    // if (HasLineOfSight(nestPosition, antTranslation.Value.xy) == false)
                    // {
                    //     proximitySteering.Value = float2.zero;
                    // }
                    // else
                    // {
                        proximitySteering.Value = nestOffset / dist;
                    
                        if (dist < foodNestRadius) 
                        {
                            loadout.Value = 0;
                        }
                    // }
                }
                else
                {
                    for (int i = 0; i < foodTranslation.Length; ++i)
                    {
                        float2 foodOffset = foodTranslation[i].Value.xy - antTranslation.Value.xy;
                        float dist = math.length(foodOffset);
                        // check line of sight
                        // if (HasLineOfSight(foodTranslation[i].Value.xy, antTranslation.Value.xy) == false)
                        // {
                        //     proximitySteering.Value = float2.zero;
                        //     continue;
                        // }

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