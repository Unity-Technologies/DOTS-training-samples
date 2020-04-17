using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct ResourceAmount : IComponentData
{
    [Range(1, 255)]
    public int Value;
}

public struct ResourceNextSpawnTime : IComponentData
{
    public double Value;
}

public struct ResourceClaimed : IComponentData
{
}