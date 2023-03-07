using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using System;
using Unity.Mathematics;

public class CarAuthoring : MonoBehaviour
{
    public float Distance;
    public float Length;
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
                Distance = authoring.Distance,
                Length = authoring.Length,
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
    public float Distance;
    public float Lane;

    public float Length;
    public float Speed;
    public float DesiredSpeed;
    public float Acceleration;

    public float4 Color;

    // immutable parameters
    public float defaultSpeed;
    public float overtakePercent;
    public float leftMergeDistance;
    public float mergeSpace;
    public float overtakeEagerness;
}