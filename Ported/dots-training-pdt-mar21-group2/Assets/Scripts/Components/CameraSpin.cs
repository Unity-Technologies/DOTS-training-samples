using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent, Serializable]
public struct CameraSpin : IComponentData
{
    public float Value;
}
