using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct ResourceAmount : IComponentData
{
    [Range(1f, 1000f)]
    public int Value;
}

public struct ResourceClaimed : IComponentData
{
}