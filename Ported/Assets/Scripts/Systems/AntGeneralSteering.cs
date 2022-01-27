using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/**
 * This system compute the general steering behavior : When Ants are loaded they seek the nest; When they are empty they Flee the nest
 */
public partial class AntGeneralSteering : SystemBase
{
    private float2 m_NestPosition;

    protected override void OnStartRunning()
    {
        var nestEntity = GetSingletonEntity<ColonyTag>();
        var nestTranslation = GetComponent<Translation>(nestEntity);
        m_NestPosition = nestTranslation.Value.xy;
    }

    protected override void OnUpdate()
    {
        float2 localNestPosition = m_NestPosition;
        Entities
            .ForEach((ref GeneralDirection generalDirection, in Loadout loadout, in Translation translation) =>
        {
            // ants flee nest if they have no resources
            generalDirection.Value = math.normalize(translation.Value.xy - localNestPosition) * (-1.0f + loadout.Value * 2.0f);
        }).Schedule();
    }

}