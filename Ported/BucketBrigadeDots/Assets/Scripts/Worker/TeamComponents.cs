
using Unity.Entities;
using Unity.Mathematics;

public struct TeamSpawner : IComponentData
{
    public Entity WorkerPrefab;
    public int NumberOfTeams;
    public int WorkersPerTeam;
}

public struct Team : IComponentData
{
    public float2 FirePosition;
    public float2 WaterPosition;
}