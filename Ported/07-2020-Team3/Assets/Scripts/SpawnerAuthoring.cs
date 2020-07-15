using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SpawnerAuthoring : IComponentData
{
    public Entity Prefab;
    public int CountX;
    public int CountY;
}
