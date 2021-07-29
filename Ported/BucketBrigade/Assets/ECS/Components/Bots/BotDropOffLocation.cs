using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct BotDropOffLocation : IComponentData
{
    public float2 Value;
}