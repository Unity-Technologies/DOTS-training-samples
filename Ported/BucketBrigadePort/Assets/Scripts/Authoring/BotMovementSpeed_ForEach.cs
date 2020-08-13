using System;
using Unity.Entities;

// ReSharper disable once InconsistentNaming
[GenerateAuthoringComponent]
public struct BotMovementSpeed_ForEach : IComponentData
{
    public float Value;
}