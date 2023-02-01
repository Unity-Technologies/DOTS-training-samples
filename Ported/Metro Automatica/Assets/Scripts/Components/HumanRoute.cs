using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

struct HumanRoute : IComponentData
{
    public NativeList<float3> BridgeRoute;
    public float3 QueuePoint;
}