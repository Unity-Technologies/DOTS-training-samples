using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct PlantAuthoring : IComponentData
{
    public Entity plantPrefab;
}