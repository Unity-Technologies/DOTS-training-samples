
using Unity.Entities;

public struct TeamSpawner : IComponentData
{
    public Entity WorkerPrefab;
    public int NumberOfTeams;
    public int WorkersPerTeam;
}

public struct Team : IComponentData
{
}

public struct TeamMembers : IBufferElementData
{
    public Entity Worker;
}