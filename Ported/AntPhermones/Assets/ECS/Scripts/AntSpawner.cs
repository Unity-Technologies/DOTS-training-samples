using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

struct AntSpawner : IComponentData
{
    public Vector3 Origin;
    public Entity AntPrefab;
    public int NbAnts;
}
