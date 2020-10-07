using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BucketSpawner : IComponentData
{
    public Entity BucketPrefab;
    
    [Min(1)]
    public int MaxBuckets;
}
