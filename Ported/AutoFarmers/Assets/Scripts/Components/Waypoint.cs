using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[InternalBufferCapacity(10)]
public struct Waypoint : IBufferElementData
{
    public int TileIndex;
}
