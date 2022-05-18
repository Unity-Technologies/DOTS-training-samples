using Unity.Entities;
using Unity.Mathematics;

public struct Commuter : IComponentData
{
    public CommuterState state;
}

public struct CommuterMovement : IComponentData
{
    public float3 destination;
    public float speed;
}

public struct CommuterConfig : IComponentData
{
    public int spawnAmount;
    public Entity commuterPrefab;
}