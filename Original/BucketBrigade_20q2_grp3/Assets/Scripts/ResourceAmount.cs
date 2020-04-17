using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct ResourceAmount : IComponentData
{
    [Range(1f, 1000f)]
    public int Value;
    [HideInInspector]
    public double NextSpawnTime;
}

public struct ResourceNextSpawnTime : IComponentData
{
    public double Value;
}

public struct ResourceClaimed : IComponentData
{
}