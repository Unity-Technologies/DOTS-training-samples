using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Board : IComponentData
{
    public Entity tilePrefab;
    public Entity wallPrefab;
    public Entity homeBasePrefab;

    [Min(2)] public int size;
    public float yNoise;

    public int wallCount;
    public int holeCount;
}
