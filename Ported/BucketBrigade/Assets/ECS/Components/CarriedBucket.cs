using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct CarriedBucket : IComponentData
{
    public Entity bucket;
}
