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
    public List<CarriageAuthoring> carriages;

    [Header("Timing Info")] 
    public float doorTransitionTime;
    public float stationWaitTime;
    public float stationWaitTimeVariance = 2f;

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

            AddComponent(new TrainScheduleInfo
            {
                baseStationWaitTime = authoring.stationWaitTime,
                doorTransisionTime = authoring.doorTransitionTime,
                waitTimeRandomVariance = authoring.stationWaitTimeVariance
            });

            AddComponent(new TargetDestination());

            AddComponent(train);

            var carriageBuffer = AddBuffer<TrainCarriage>();
            foreach (var carriageAuthoring in authoring.carriages)
            {
                carriageBuffer.Add(new TrainCarriage()
                {
                    CarriageNumber = carriageAuthoring.CarriageNumber,
                    CarriageEntity = GetEntity(carriageAuthoring.gameObject)
                });
            }
        }
        
    }
}

public struct Train : IComponentData
{
    public Entity entity;
    public Entity Line;
    public Entity currentPlatform;
    public TrainState State;
    public int trainID;
    public int nextStationIndex;
    public int direction; //direction of travel -1 or 1
    public float Speed;
}

public struct TrainScheduleInfo : IComponentData
{

    public float baseStationWaitTime;
    public float stationWaitTime;
    public float doorTransisionTime;

    public float waitTimeRandomVariance;
    
    public float timeInState;
}

public struct TrainCarriage : IBufferElementData
{
    public int CarriageNumber;
    public Entity CarriageEntity;
}