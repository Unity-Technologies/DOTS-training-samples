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
    
    protected override void OnStartRunning()
    {
        m_FoodQuery = GetEntityQuery(ComponentType.ReadOnly<ResourceTag>(), ComponentType.ReadOnly<Translation>());
        var nestEntity = GetSingletonEntity<ColonyTag>();
        var nestTranslation = GetComponent<Translation>(nestEntity);
        m_NestPosition = nestTranslation.Value.xy;
    }

    protected override void OnUpdate()
    {
        // First gather all active food
        NativeArray<Translation> foodTranslation = m_FoodQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        float2 nestPosition = m_NestPosition;
        
        // We may sort the array of food and optimize the Food looping
        Entities.WithReadOnly(foodTranslation)
            .ForEach((ref ProximitySteering proximitySteering, ref Loadout loadout, in Translation antTranslation) =>
            {
                if (loadout.Value > 0)
                {
                    float2 nestOffset = nestPosition - antTranslation.Value.xy;
                    float sqDist = math.lengthsq(nestOffset);
                    proximitySteering.Value = nestOffset / sqDist;
                    
                    if (sqDist < 0.1f / 128f) 
                    {
                        loadout.Value = 0;
                    }
                }
                else
                {
                    for (int i = 0; i < foodTranslation.Length; ++i)
                    {
                        float2 foodOffset = foodTranslation[i].Value.xy - antTranslation.Value.xy;
                        float sqDist = math.lengthsq(foodOffset);
                        // check line of sight
                        // if (HasLineOfSight(foodTranslation[i], antTranslation) == false)
                        // {
                        //     continue;
                        // }

                        proximitySteering.Value = foodOffset / sqDist;
                        
                        // tHis code handle the loading / unloading
                        if (math.lengthsq(foodOffset) < 0.1f / 128f) 
                        {
                            loadout.Value = 1;
                        }
                        break;
                    }
                }
            }).WithDisposeOnCompletion(foodTranslation).ScheduleParallel();
    }

    private bool HasLineOfSight(Translation translationA, Translation translationB)
    {
        return false;
    }
}