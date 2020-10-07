using System;
using Unity.Entities;
using Unity.Mathematics;

public struct GameInfo : IComponentData
{
    public int2 boardSize;
}
