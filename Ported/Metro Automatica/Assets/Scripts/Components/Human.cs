using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

struct Human : IComponentData
{
    public int TrainID;
    public float3 currentDestination;
    public Entity WagonOfChoice;
    public float3 QueuePoint;
}
[InternalBufferCapacity(4)]
struct BridgeRouteWaypoint : IBufferElementData
{
    public float3 Value;
}