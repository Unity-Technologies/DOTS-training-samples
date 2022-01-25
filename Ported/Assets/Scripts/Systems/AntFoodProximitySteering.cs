using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/**
 * This system compute the food final approach steering behavior : Ants will seek food source when they are in food range, with clear line of sight.
 */
public partial class AntFoodProximitySteering : SystemBase
{
    private EntityQuery m_FoodQuery;
    private float m_ProximityRangeSqr = 25.0f;
    
    protected override void OnStartRunning()
    {
        m_FoodQuery = GetEntityQuery(ComponentType.Exclude<Velocity>(), ComponentType.ReadOnly<ResourceTag>());
        // m_ProximityRangeSqr = Query general config
    }

    protected override void OnUpdate()
    {
        // First gather all active food
        NativeArray<Translation> foodTranslation = m_FoodQuery.ToComponentDataArray<Translation>(Allocator.Temp);
        float proximityRangeSqr = m_ProximityRangeSqr;
        
        // We may sort the array of food and optimize the Food looping
        
        Entities
            .WithAll<GeneralDirection>()
            .WithNone<ResourceTag>()
            .ForEach((Entity entity, ref FoodProximitySteering foodProximitySteering, in Translation antTranslation) =>
            {
                foodProximitySteering.Value = float2.zero;
                
                for (int i = 0; i < foodTranslation.Length; ++i)
                {
                    float2 foodOffset = foodTranslation[i].Value.xy - antTranslation.Value.xy;

                    // check X Z range
                    if (math.lengthsq(foodOffset) > proximityRangeSqr)
                    {
                        continue;
                    }
            
                    // check line of sight
                    if (LineOfSight(foodTranslation[i], antTranslation) == false)
                    {
                        continue;
                    }
            
                    // steer toward the food
                    foodProximitySteering.Value = math.normalize(foodTranslation[i].Value.xy - antTranslation.Value.xy);
                    break;
                }
            }).WithoutBurst().Run();

        foodTranslation.Dispose();
    }

    private bool LineOfSight(Translation translationA, Translation translationB)
    {
        return true;
    }
}