using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Target : IComponentData
{
    public float3 flameCellPosition;
    public float3 waterCellPosition;
}
