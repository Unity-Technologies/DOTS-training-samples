﻿using System;
using Unity.Entities;
using Unity.Mathematics;

public struct RockVelocityComponentData: IComponentData
{
    public float3 value;

    public static implicit operator RockVelocityComponentData(float3 velocity) => new RockVelocityComponentData()
    {
        value = velocity
    };

    public static implicit operator float3(RockVelocityComponentData c) => c.value;
}

public struct DebugRockGrabbedTag : IComponentData
{
    
}
 
 public struct RockTag : IComponentData
 {
    
 }

 public struct RockBounds : IComponentData
 {
     public float2 range;
 }

public struct RockReservedTag : IComponentData
{
}

public struct RockRadiusComponentData: IComponentData
{
    public float value;

    public static implicit operator RockRadiusComponentData(float rad) => new RockRadiusComponentData()
    {
        value = rad
    };

    public static implicit operator float(RockRadiusComponentData c) => c.value;
}

