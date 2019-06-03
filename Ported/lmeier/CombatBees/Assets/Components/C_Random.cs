using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct C_Random : IComponentData
{
    public Unity.Mathematics.Random Generator;
}
