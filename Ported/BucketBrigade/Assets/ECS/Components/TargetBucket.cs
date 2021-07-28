using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TargetBucket : IComponentData
{
    public Entity bucket;
}
