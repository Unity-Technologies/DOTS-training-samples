using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using System;
using Unity.Mathematics;

public class CarAuthoring : MonoBehaviour
{
   
//    public float Speed;
  //  public float DesiredSpeed;
   // public float Acceleration;
    //public float4 Color;

    class Baker : Baker<CarAuthoring>
    {
        public override void Bake(CarAuthoring authoring)
        {
            AddComponent(new CarData
            {
            });
            AddComponent(new CarColor
            {
            });
            AddComponent(new CarParameters());
        }
    }
}

public struct CarData : IComponentData
{
    public float Distance;
    public float Lane;
    public float Speed;
    public float DesiredLane;

    public float TEMP_NextLaneChangeCountdown;

    
}
public struct CarColor : IComponentData
{
    public float PreviousDifferential;
}

public readonly struct CarParameters : IComponentData
{
    // immutable parameters
    public readonly float defaultSpeed;
    public readonly float overtakePercent;
    public readonly float leftMergeDistance;
    public readonly float mergeSpace;
    public readonly float overtakeEagerness;
    public readonly float Length;
    public readonly float DesiredSpeed;
    public readonly float Acceleration;

    public CarParameters(float aDefaultSpeed, float anOvertakePercent, float aLeftMergeDistance, float aMergeSpace, float anOvertakeEagernessv4,float aLenght,float aDesiredSpeed,float anAcceleration) 
    {
        defaultSpeed = aDefaultSpeed;
        overtakePercent = anOvertakePercent;
        leftMergeDistance = aLeftMergeDistance;
        mergeSpace = aMergeSpace;
        overtakeEagerness = anOvertakeEagernessv4;
        Length = aLenght;
        DesiredSpeed = aDesiredSpeed;
        Acceleration = anAcceleration;
    }
}