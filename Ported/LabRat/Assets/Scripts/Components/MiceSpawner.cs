using Unity.Entities;
using Unity.Mathematics;

public struct MiceSpawner : IComponentData
{
    public float SpawnCooldown;
    public float SpawnCounter;
    public int RemainingMiceToSpawn;
    public uint RandomizerState;
}
