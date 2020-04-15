using Unity.Entities;

public struct AgentSpawner : IComponentData
{
    public Entity Prefab;
    public int NumAgents;
}
