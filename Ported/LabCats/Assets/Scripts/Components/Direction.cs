using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public enum Dir
{
    Up,
    Down,
    Left,
    Right
}

[GenerateAuthoringComponent]
public struct Direction : IComponentData
{
    public Dir Value;
}
