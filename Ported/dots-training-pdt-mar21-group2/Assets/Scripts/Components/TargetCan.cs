using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
// For hand to mark which hand it will target
public struct TargetCan : IComponentData
{
    public Entity Value;
}
