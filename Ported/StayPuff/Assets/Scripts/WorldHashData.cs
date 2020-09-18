using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct WorldHashData : IComponentData
{
    public int2 gridSteps;
    public float cellSize;
}
