using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct UnitSpawnerComponent : IComponentData
{
    public int max;
    public float frequency;
    public float counter;


    //public Prefab spawnObject;
}
