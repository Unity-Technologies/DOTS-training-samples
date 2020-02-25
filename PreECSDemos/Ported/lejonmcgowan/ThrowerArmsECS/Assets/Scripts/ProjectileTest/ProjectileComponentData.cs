﻿using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct ProjectileComponentData : IComponentData
{
    public float Lifetime;
    
    public float3 initPosition;
    public float3 Velocity;
    public float TimeLeft;
}