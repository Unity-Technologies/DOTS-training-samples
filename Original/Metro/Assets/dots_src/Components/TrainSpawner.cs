using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct TrainSpawner : IComponentData
{
    public Entity TrainPrefab;
    public Entity CarriagePrefab;
}
