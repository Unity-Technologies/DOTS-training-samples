using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct Target : IComponentData
{
    public float2 flameCellPosition;
    public float2 waterCellPosition;
}
