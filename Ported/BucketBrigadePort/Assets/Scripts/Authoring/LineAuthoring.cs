using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Line : IComponentData
{
    public int lineId;
    public float3 fillTranslation;
    public float3 tossTranslation;
    // public Entity[] bots;
}
