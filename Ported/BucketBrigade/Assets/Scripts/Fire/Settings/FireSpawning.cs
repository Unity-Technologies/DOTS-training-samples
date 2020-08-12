using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct FireSpawning : IComponentData
{
    public Entity Prefab;
}
