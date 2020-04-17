using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct LocalTranslation : IComponentData
{
    public float2 Value;
}
