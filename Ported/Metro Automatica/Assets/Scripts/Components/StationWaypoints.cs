using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

struct StationWaypoints : IComponentData
{
    public List<List<float3>> QueuePoints;
    public List<List<float3>> BridgePathWays;
}