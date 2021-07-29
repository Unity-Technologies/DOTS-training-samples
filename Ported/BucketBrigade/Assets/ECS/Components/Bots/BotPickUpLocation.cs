using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct BotPickUpLocation : IComponentData
{
    public float2 Value;
}