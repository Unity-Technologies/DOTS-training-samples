using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct CurrentScale : IComponentData
{
    public float Value;
}
