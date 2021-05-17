using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class Bounds : IComponentData
{
    public AABB Value;
}
