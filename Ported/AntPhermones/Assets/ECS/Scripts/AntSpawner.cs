using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

struct AntSpawner : IComponentData
{
    public Vector3 Origin;
    public Vector3 ColonyPosition;
    public Entity AntPrefab;
    public Entity ColonyPrefab;
    public int NbAnts;
}
