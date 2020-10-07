using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public enum EntityDirection
{
    Up,
    Right,
    Down,
    Left
}

[GenerateAuthoringComponent]
public struct Direction : IComponentData
{
    public EntityDirection Value;

    public float3 GetVelocity()
    {
        if (Value == EntityDirection.Right)
        {
            return new float3(1,0,0);
        }
        if (Value == EntityDirection.Up)
        {
            return  new float3(0,0,1);
        }
        
        return new float3(0,0,0);
    }
}
// 0->Right
// 1->Up
// 2->Left
// 3->Down