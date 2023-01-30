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
    public int CarriageCount;
    public float Speed;
    
    //TODO Remove - just for debugging
    public float3 targetDestTest;

    class Baker : Baker<TrainAuthoring>
    {
        public override void Bake(TrainAuthoring authoring)
        {
            AddComponent(new Train()
            {
                CarriageCount = authoring.CarriageCount,
                Speed = authoring.Speed
            });
            
            AddComponent(new TargetDestination()
            {
                TargetPosition = authoring.targetDestTest
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