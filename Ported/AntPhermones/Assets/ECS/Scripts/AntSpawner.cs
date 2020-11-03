using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

struct AntSpawner : IComponentData
{
    public Vector3 Origin;
    public Vector3 ColonyPosition;
    public Vector3 FoodPosition;
    public Entity AntPrefab;
    public Entity ColonyPrefab;
    public Entity FoodPrefab;
    public int NbAnts;
}
