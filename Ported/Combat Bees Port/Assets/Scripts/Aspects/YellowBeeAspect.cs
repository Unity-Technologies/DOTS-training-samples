using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

// Instead of directly accessing the Turret component, we are creating an aspect.
// Aspects allows you to provide a customized API for accessing your components.
readonly partial struct YellowBeeAspect : IAspect<YellowBeeAspect>
{
    // This reference provides read only access to BeeAspect
    // Trying to use ValueRW (instead of ValueRO) on a read-only reference is an error.
    readonly RefRO<YellowTeam> m_tag;
}
