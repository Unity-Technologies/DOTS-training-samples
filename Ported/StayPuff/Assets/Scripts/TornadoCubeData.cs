using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TornadoCubeData : IComponentData
{
    public Entity Prefab;
    public int CountX;
    public int CountZ;
    public Vector3 BoundsMax;
    public Vector3 BoundsMin;
}