using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// tag component
public struct Bucket : IComponentData
{
    public const float MaxVolume = 3.0f;
}

public struct CarryableObject : IComponentData
{
    public Entity CarryingEntity;
}