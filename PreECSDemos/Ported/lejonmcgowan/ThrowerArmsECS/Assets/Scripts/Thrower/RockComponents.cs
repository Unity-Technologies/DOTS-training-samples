﻿using System;
using Unity.Entities;
using Unity.Mathematics;
 using Unity.Transforms;

 public struct RockVelocityComponentData: IComponentData
{
    public float3 value;

    public static implicit operator RockVelocityComponentData(float3 velocity) => new RockVelocityComponentData()
    {
        value = velocity
    };

    public static implicit operator float3(RockVelocityComponentData c) => c.value;
}

public struct RockGrabbedTag : IComponentData
{
    
}
 
 public struct RockTag : IComponentData
 {
    
 }

 public struct RockDestroyBounds : IComponentData
 {
     public float2 Value;
 }
 
 public struct RockSpawnerBounds : IComponentData
 {
     public float2 Value;
 }

public struct RockReservedTag : IComponentData
{
}

 public struct RockReserveRequest : IComponentData
 {
     public Entity armRef;
     public Entity rockRef;
     
     //used for tie breaking purposes
     public float3 armPos;
     
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

