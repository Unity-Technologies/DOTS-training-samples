using Unity.Entities;
using Unity.Mathematics;
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
    public int CarriageCount;
    public float Speed;

    [Header("Timing Info")] 
    public float doorTransitionTime;
    public float stationWaitTime;

    class Baker : Baker<TrainAuthoring>
    {
        public override void Bake(TrainAuthoring authoring)
        {
            AddComponent(new Train()
            {
                CarriageCount = authoring.CarriageCount,
                Speed = authoring.Speed
            });

            AddComponent(new TrainScheduleInfo(
                authoring.doorTransitionTime,
                authoring.stationWaitTime));

            AddComponent(new TargetDestination()
            {
            });
        }
    }
}

public struct Train : IComponentData
{
    public Entity Line;
    public TrainState State;
    public int CarriageCount;
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