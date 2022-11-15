using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public enum TileType
{
    Obstacle,
    Road
}

public struct MapData : IComponentData
{
    public int Rows;
    public int Columns;
    public NativeArray<TileType> TileTypes;
    public NativeArray<byte> StrengthList;
}
