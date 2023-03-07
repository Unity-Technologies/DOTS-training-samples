using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using System;
using Unity.Mathematics;

public class CarAuthoring : MonoBehaviour
{
   
    public float Speed;
    public float DesiredSpeed;
    public float Acceleration;
    public float4 Color;

    class Baker : Baker<CarAuthoring>
    {
        public override void Bake(CarAuthoring authoring)
        {
            AddComponent(new Car
            {
                Speed = authoring.Speed,
                DesiredSpeed = authoring.DesiredSpeed,
                Acceleration = authoring.Acceleration,
                Color = authoring.Color
            });
            
        }
    }
}

public struct Car : IComponentData
{
    public float Speed;
    public float DesiredSpeed;
    public float Acceleration;
    public float4 Color;
}