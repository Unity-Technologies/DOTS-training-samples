using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct TornadoSpawner : IComponentData
{
    public Entity particlePrefab;
    public int particleCount;
}
