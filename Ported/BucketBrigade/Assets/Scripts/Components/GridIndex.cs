using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
[GenerateAuthoringComponent]
public struct GridIndex : IComponentData
{
    public Unity.Mathematics.int2 Index;
}
