using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TrainSpawner : IComponentData
{
    public Entity CarriagePrefab;
    public Entity RailPrefab;
    
    public int TrainsPerLine;
}
