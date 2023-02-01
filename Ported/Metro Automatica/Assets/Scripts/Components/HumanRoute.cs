using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

struct HumanRoute : IComponentData
{
    public List<float3> BridgeRoute;
    public float3 QueuePoint;
}