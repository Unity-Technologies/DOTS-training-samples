using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[InternalBufferCapacity(1024)]
public struct Pheromone : IBufferElementData
{
    public float Value;
}