using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SpawnCount : IComponentData
{
    public int TotalCount;
}