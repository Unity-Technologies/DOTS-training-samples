using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Target : IComponentData
{
    public Entity Value;
}
