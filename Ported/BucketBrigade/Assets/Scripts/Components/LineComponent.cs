using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
[GenerateAuthoringComponent]
public struct LineComponent : IComponentData
{
    public Entity start;
    public Entity end;
}