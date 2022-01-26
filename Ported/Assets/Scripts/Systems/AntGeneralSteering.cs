using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

/**
 * This system compute the general steering behavior : When Ants are loaded they seek the nest; When they are empty they Flee the nest
 */
public partial class AntGeneralSteering : SystemBase
{
    private float2 m_NestPosition;
    private float goalSteerStrength;

    protected override void OnStartRunning()
    {
        // We will ultimately gather Nest Position here
        // m_NestPosition = 
    }

    protected override void OnUpdate()
    {
        float2 localNestPosition = m_NestPosition;
        Entities
            .ForEach((Entity entity, ref GeneralDirection generalDirection, in Loadout loadout, in Translation translation) =>
        {
            // ants flee nest if they have no resources
            generalDirection.Value = math.normalize(translation.Value.xy - localNestPosition) * (-1.0f + loadout.Value * 2.0f);
        }).Schedule();
    }

}