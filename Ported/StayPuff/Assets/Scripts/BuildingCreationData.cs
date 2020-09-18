using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct BuildingCreationData : IComponentData
{
    public int height;
    public Entity prefab;
    public Entity horizontal;
    public Entity horizontalthin;
    public Entity vertical;
    public float jointBreakDistance;
}
