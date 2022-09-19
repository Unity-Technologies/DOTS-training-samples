using Unity.Entities;
using UnityEngine;

public struct TileBufferElement : IBufferElementData
{
    public bool DownWall;
    public bool LeftWall;
    public bool RightWall;
    public bool UpWall;

    public bool TempVisited;
}
