using System;
using Unity.Entities;

public struct PlacingArrow : IComponentData
{
    public int TileIndex;
    public byte Direction;
}
