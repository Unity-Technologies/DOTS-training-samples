using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct BeeSpawner : IComponentData
{
    public Entity beePrefab;
    public int numBeesToSpawn;
    public int teamNumber;

}
