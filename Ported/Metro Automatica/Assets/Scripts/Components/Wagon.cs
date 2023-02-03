using Unity.Entities;
using Unity.Mathematics;

struct Wagon : IComponentData
{
    public Entity WagonPrefab;
    public int TrainID;
    public int Direction;
    // public float3 currentDestination;
    public float WagonOffset;
    public int StationCounter;
    public float StopTimer;
}
struct StationWayPoints : IBufferElementData
{
    public float3 Value;
}