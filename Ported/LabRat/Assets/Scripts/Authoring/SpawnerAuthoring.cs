using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Spawner : IComponentData
{
    public Entity Prefab;
    public float Max;
    [HideInInspector] public float Counter;
    public float Frequency;
    [HideInInspector] public int TotalSpawned;
    
    [Header("Speed")]
    public float MinSpeed;
    public float MaxSpeed;

    [Header("Scale")]
    public float Scale;
    public bool Enable;
    public float MinScale;
    public float MaxScale;
}