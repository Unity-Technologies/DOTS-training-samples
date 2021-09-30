using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TrainSpawner : IComponentData
{
    public Entity TrainPrefab;
    public Entity CarriagePrefab;
}
