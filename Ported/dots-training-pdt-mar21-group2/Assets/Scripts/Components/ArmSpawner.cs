using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public struct Spawner : IComponentData
{
    public Entity ArmPrefab;
    public uint ArmCount;
}