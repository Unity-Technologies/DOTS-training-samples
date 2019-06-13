using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct C_GridIndex : IComponentData
{
    public int x;
    public int y;
}
