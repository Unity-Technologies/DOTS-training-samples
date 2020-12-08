using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct BeeSpawnRequest : IComponentData
{
    public int numOfBeesToSpawn;
}
