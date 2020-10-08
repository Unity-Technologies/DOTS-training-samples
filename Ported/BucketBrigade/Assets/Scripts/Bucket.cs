using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

// tag component
public struct Bucket : IComponentData
{
}

public struct CarryableObject : IComponentData
{
    public Entity CarryingEntity;
}