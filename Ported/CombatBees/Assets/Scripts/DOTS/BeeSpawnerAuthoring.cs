using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BeeSpawner : IComponentData
{
    public Entity Prefab;
    public int BeeCount;
}