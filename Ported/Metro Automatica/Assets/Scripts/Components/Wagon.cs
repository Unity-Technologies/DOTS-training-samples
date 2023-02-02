using Unity.Entities;
using Unity.Mathematics;

struct Wagon : IComponentData
{
    public Entity WagonPrefab;
    public int TrainID;
    public int Direction;
    // public float3 currentDestination;
    public float TrainOffset;
    public int StationCounter;
}
struct StationWayPoints : IBufferElementData
{
    public float3 Value;
}