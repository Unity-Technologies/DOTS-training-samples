using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

struct StationWaypoints : IComponentData
{
    public NativeList<float3> QueuePoints1;
    public NativeList<float3> QueuePoints2;
    public NativeList<float3> BridgePathWays1;
    public NativeList<float3> BridgePathWays2;
}