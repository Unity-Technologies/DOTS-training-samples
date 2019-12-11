using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct HandMatrix : IComponentData
{
    public Matrix4x4 Value;
}

