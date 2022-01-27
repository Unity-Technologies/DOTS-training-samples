using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/**
 * This system compute the food final approach steering behavior : Ants will seek food source when they are in food range, with clear line of sight.
 */
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
        Entities
            .ForEach((ref ProximitySteering proximitySteering, in Loadout loadout, in Translation antTranslation) =>
            {
                if (loadout.Value > 0)
                {
                    proximitySteering.Value = math.normalize(nestPosition - antTranslation.Value.xy);
                }
                else
                {
                    for (int i = 0; i < foodTranslation.Length; ++i)
                    {
                        float2 foodOffset = foodTranslation[i].Value.xy - antTranslation.Value.xy;
            
                        // check line of sight
                        if (HasLineOfSight(foodTranslation[i], antTranslation) == false)
                        {
                            continue;
                        }

                        proximitySteering.Value = math.normalize(foodTranslation[i].Value.xy - antTranslation.Value.xy);
                        
                        // tHis code handle the loading / unloading
                        // if ((ant.position - targetPos).sqrMagnitude < 4f * 4f) {
                        //     ant.holdingResource = !ant.holdingResource;
                        // }
                        break;
                    }
                }
            }).WithDisposeOnCompletion(foodTranslation).Schedule();
    }

    private bool HasLineOfSight(Translation translationA, Translation translationB)
    {
        return false;
    }
}