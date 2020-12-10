using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BarSpawner : IComponentData
{
    public Entity barPrefab;
    public float4 color;
    [Range(0, 300)]
    public int buildingsCount;
    [Range(0, 50)]
    public int groundDetailsCount;
}
