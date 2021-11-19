using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
[Serializable]
public struct Decay : IComponentData
{
    public float DecayTimeRemaing;
    public float3 originalScale;
}
