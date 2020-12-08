using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct BeeSpawner : IComponentData
{
    public float3 position;
    public float radius;

    public Entity beePrefab;
    public int numBeesToSpawn;
    public int teamNumber;
    public float4 teamColour;
}
