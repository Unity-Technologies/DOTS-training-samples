using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct BuildingCreationData : IComponentData
{
    public int height;
    public Entity prefab;
}
