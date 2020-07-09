using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct CellRect : IComponentData
{
    public int X;
    public int Y;
    public int Width;
    public int Height;
}
