using Unity.Entities;
using Unity.Mathematics;


struct PlayerData : IComponentData
{
    public float3 position;
    public float2 direction;
    public float speed;
    public int StartXIndex;
    public int StartYIndex;
    public float3 spawnerPos;
    public int PillsCollected;
}

