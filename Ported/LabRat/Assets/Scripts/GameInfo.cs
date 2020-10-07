using System;
using Unity.Entities;
using Unity.Mathematics;

public struct GameInfo : IComponentData
{
    public int2 boardSize;
    public Entity ArrowPrefab;
}

//Possibly move this struct to its own file. 
public struct TileWall : IBufferElementData
{
    public byte Value;
}
