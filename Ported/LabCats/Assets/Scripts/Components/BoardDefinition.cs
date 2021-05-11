using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BoardDefinition : IComponentData
{
    public float CellSize;
    public int NumberColumns;
    public int NumberRows;
}
