using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public struct Line : IComponentData
{
    public Entity EmptyHead;
    public Entity FullHead;
    public Entity EmptyTail;
    public Entity FullTail;

    public int HalfCount;
}