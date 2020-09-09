using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

struct Speed : IComponentData
{
    public float Value;
}

[GenerateAuthoringComponent]
public struct SpeedAuthoring : IComponentData
{
    public float MinSpeed;
    public float MaxSpeed;
}

