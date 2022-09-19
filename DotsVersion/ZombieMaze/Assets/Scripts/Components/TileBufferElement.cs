using Unity.Entities;
using UnityEngine;

public struct TileBufferElement : IBufferElementData
{
    public bool DownWall;
    public bool LeftWall;
}
