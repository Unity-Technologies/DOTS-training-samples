using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Bounds : IComponentData
{
    public AABB Value;
}
