using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Speed : IComponentData
{
    public float Value;
}
