using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Decay : IComponentData
{
    public float Rate;
}
