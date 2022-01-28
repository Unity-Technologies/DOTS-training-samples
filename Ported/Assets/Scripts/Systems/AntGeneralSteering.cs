using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/**
 * This system compute the general steering behavior : When Ants are loaded they seek the nest; When they are empty they Flee the nest
 */
[UpdateBefore(typeof(SteeringSystem))]
public partial class AntGeneralSteering : SystemBase
{
    private float2 m_NestPosition;
    private float m_MapSize = 0.5f;
    private float m_ColonySize = 0.5f;

    protected override void OnStartRunning()
    {
        var nestEntity = GetSingletonEntity<ColonyTag>();
        var nestTranslation = GetComponent<Translation>(nestEntity);
        var colonyEntity = GetSingletonEntity<ColonyTag>();
        var scale = GetComponent<NonUniformScale>(colonyEntity);
        m_MapSize = Configuration.MapSizeWorld;
        m_ColonySize = scale.Value.x;
        m_NestPosition = nestTranslation.Value.xy;
    }

    protected override void OnUpdate()
    {
        float2 localNestPosition = m_NestPosition;
        float mapSize = m_MapSize;
        Entities
            .ForEach((ref GeneralDirection generalDirection, in Velocity velocity, in Loadout loadout, in Translation translation) =>
        {
            float2 offsetToNest = localNestPosition - translation.Value.xy;
            float distToNest = math.length(offsetToNest);
            float2 nestDirection = offsetToNest / distToNest;
            if (loadout.Value == 0)
            {
                //float headingToNest = math.dot(nestDirection, velocity.Direction);
                float proximityToNest = 1.0f - (distToNest / mapSize);
                generalDirection.Value = (-offsetToNest / distToNest)  * proximityToNest;
            }
            else
            { 
                //float headingFromNest = math.dot(nestDirection, velocity.Direction);
                float proximityToNest =  distToNest / mapSize;
                generalDirection.Value = (offsetToNest / distToNest) /** headingFromNest*/ * proximityToNest;
            }
        }).ScheduleParallel();
    }

}