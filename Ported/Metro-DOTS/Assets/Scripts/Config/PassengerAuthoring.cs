using Components;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class PassengerAuthoring : MonoBehaviour
{
    public float MinHeight;
    public float MaxHeight;
    class Baker : Baker<PassengerAuthoring>
    {
        public override void Bake(PassengerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PassengerComponent
            {
                Color = new float3(1, 0, 1),
                Height = authoring.MaxHeight
            });
            AddComponent(entity, new PassengerOnboarded());
            AddComponent(entity, new PassengerOffboarded());
            AddComponent(entity, new PassengerWalkingToQueue());
            AddComponent(entity, new PassengerWalkingToSeat());
            AddComponent(entity, new PassengerWalkingToDoor());
            AddComponent(entity, new PassengerWaitingToExit());
            AddComponent(entity, new PassengerTravel());
            SetComponentEnabled<PassengerOnboarded>(entity, false);
            SetComponentEnabled<PassengerOffboarded>(entity, false);
            SetComponentEnabled<PassengerWalkingToQueue>(entity, false);
            SetComponentEnabled<PassengerWalkingToSeat>(entity, false);
            SetComponentEnabled<PassengerWalkingToDoor>(entity, false);
            SetComponentEnabled<PassengerWaitingToExit>(entity, false);
        }
    }
}

public struct PassengerComponent : IComponentData
{
    public float3 Color;
    public float Height;
    public int TrainId;
    public float3 SeatPosition;
    public float3 ExitPosition;
    public int SeatIndex;
    public float3 RelativePosition;
    public bool HasSeat;
    public bool MoveToPosition;
    public float3 TargetPosition;
    public float3 InitialPosition;
    public float T;
    public bool HasJustUnloaded;
}

