using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent, Serializable]
public struct CarouselSpawner : IComponentData
{
    public Entity SpawnPrefab;
    public float Distance;
    public float Frequency;
}
