using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using System;
using Unity.Mathematics;

public class CarAuthoring : MonoBehaviour
{
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
    public float CurrentLane;
    public float Speed;
    public float OvertakeModeCountdown;
    public float OvertakeModeReturnToLane;
    public float DesiredLane;
    public float Acceleration;
    public float DesiredSpeed;
    public float PreviousDifferential;
    public float CurrentDifferential;
    public float TEMP_NextLaneChangeCountdown;    
}

public struct CarColor : IComponentData
{
    public float CurrentDifferential;
    public float PreviousDifferential;
}

public readonly struct CarParameters : IComponentData
{
    public readonly float defaultSpeed;
    public readonly float overtakePercent;
    public readonly float leftMergeDistance;
    public readonly float mergeSpace;
    public readonly float overtakeEagerness;
    public readonly float Length;
    public readonly float DesiredSpeed;
    public readonly float Acceleration;
    public readonly float CruisingSpeed;
    public readonly float OvertakeSpeed;

    public CarParameters(float aDefaultSpeed, float anOvertakePercent, float aLeftMergeDistance, float aMergeSpace, float anOvertakeEagernessv4,float aLenght,float aDesiredSpeed,float anAcceleration,float aCruisingSpeed,float anOvertakeSpeed) 
    {
        defaultSpeed = aDefaultSpeed;
        overtakePercent = anOvertakePercent;
        leftMergeDistance = aLeftMergeDistance;
        mergeSpace = aMergeSpace;
        overtakeEagerness = anOvertakeEagernessv4;
        Length = aLenght;
        DesiredSpeed = aDesiredSpeed;
        Acceleration = anAcceleration;
        CruisingSpeed = aCruisingSpeed;
        OvertakeSpeed =anOvertakeSpeed;
    }
}