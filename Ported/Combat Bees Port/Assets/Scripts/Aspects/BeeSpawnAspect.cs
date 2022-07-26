using Components;
using Unity.Entities;


// Instead of directly accessing the Turret component, we are creating an aspect.
// Aspects allows you to provide a customized API for accessing your components.
readonly partial struct BeeSpawnAspect : IAspect<BeeSpawnAspect>
{
    // This reference provides read only access to the Turret component.
    // Trying to use ValueRW (instead of ValueRO) on a read-only reference is an error.
    readonly RefRO<BeeSpawnData> m_SpawnData;

    // Note the use of ValueRO in the following properties.
    public Entity BlueBeePrefab => m_SpawnData.ValueRO.blueBeePrefab;
    public Entity YellowBeePrefab => m_SpawnData.ValueRO.yellowBeePrefab;
}
