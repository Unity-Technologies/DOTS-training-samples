using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct TileRenderer : IComponentData
{
    public int2 tile;
}
