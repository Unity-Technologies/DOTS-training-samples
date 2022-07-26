using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
[GenerateAuthoringComponent]
public struct Map : IComponentData
{
    public int2 mapSize;
    public Entity cellPrefab;
}
