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
        // We will ultimately gather Nest Position here
        // m_NestPosition = 
    }

    protected override void OnUpdate()
    {
        float2 localNestPosition = m_NestPosition;
        Entities
            .WithAll<GeneralDirection>()
            .WithNone<ResourceTag>()
            .ForEach((Entity entity, ref GeneralDirection generalDirection, in Translation translation) =>
        {
            // ants flee nest if they have no resources
            generalDirection.Value = math.normalize(translation.Value.xy - localNestPosition);
        }).Schedule();
        Entities
            .WithAll<GeneralDirection, ResourceTag>()
            .ForEach((Entity entity, ref GeneralDirection generalDirection, in Translation translation) =>
            {
                // ants seek nest if they have resources
                generalDirection.Value = math.normalize(localNestPosition - translation.Value.xy);
            }).Schedule();
    }

}