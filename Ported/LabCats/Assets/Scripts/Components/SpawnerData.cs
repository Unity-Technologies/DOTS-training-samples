using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

// ???
public enum SpawnerType
{
    MouseSpawner,
    MouseAndCatSpawner,
    CatSpawner
}

public struct SpawnerData : IComponentData
{
    public float Frequency;
    public Dir Direction;
    public SpawnerType Type;
}
