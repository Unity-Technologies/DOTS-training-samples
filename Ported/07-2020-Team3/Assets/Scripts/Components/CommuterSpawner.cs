using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct CommuterSpawner : IComponentData
{
    public Entity CommuterPrefab;
    public int SpawnCount;
    public float SpawnRadius;
    public float MinCommuterSpeed;
    public float MaxCommuterSpeed;
}
