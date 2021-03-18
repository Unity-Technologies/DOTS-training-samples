using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent, Serializable]
public struct CameraSwayX : IComponentData
{
    public float Value;
}
