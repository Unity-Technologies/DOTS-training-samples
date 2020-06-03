using Unity.Entities;

public struct AgentSpawner : IComponentData
{
    public Entity Prefab;
    public int NumAgents;
    public float MaxSpeed;
    public float MinSpeed;
    public float OvertakeIncrement;
}
