using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PlatformSpawner : IComponentData
{
    public Entity LinePrefab;
    public Entity PlatformPrefab;
}