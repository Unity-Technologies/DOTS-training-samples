using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TornadoSpawnData : IComponentData
{    
    public Entity Prefab;
    public int TornadoCount;

}