using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public enum TrainState
{
    Idle,
    DoorOpening,
    DoorClosing,
    Boarding,
    TrainMovement
}

public class TrainAuthoring : MonoBehaviour
{
    [Header("Train Info")] 
    public GameObject carridgePrefab;
    public float Speed;

    [Header("Timing Info")] 
    public float doorTransitionTime;
    public float stationWaitTime;

    public GameObject line;

    public static int trainID = 0;

    class Baker : Baker<TrainAuthoring>
    {
        public override void Bake(TrainAuthoring authoring)
        {
            Train train = new Train()
            {
                Speed = authoring.Speed,
                entity = GetEntity(authoring.gameObject),
                trainID = trainID++,
                Line = GetEntity(authoring.line)
            };

            AddComponent(new TrainScheduleInfo(
                authoring.doorTransitionTime,
                authoring.stationWaitTime));

            AddComponent(new TargetDestination());

            AddComponent(train);
        }
        
    }
}

public struct Train : IComponentData
{
    public Entity entity;
    public Entity Line;
    public Entity currentStation;
    public TrainState State;
    public int trainID;
    public int nextStationIndex;
    public int direction; //direction of travel -1 or 1
    public float Speed;
}

public struct TrainScheduleInfo : IComponentData
{
    public TrainScheduleInfo(float doorTransisionTime, float stationWaitTime)
    {
        this.doorTransisionTime = doorTransisionTime;
        this.stationWaitTime = stationWaitTime;
        this.timeInState = 0f;
    }
    
    public readonly float stationWaitTime;
    public readonly float doorTransisionTime;

    public float timeInState;
}