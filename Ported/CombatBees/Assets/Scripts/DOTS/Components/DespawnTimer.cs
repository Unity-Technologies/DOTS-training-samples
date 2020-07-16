using Unity.Entities;

public struct DespawnTimer : IComponentData
{
    public float Time;
}


// HACK

public struct KillBee : IComponentData
{
    public float Time;
}