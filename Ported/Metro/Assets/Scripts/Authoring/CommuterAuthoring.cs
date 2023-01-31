using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public enum CommuterState
{
    Idle,
    InTrain,
    Boarding,
    Unboarding,
    MoveToDestination,
    Queueing
}

public class CommuterAuthoring : MonoBehaviour
{
    public GameObject CurrentPlatform; // TODO: remove, this should be set from the spawning logic
    public float Velocity = 5f; // TODO: remove, this should be set from the spawning logic
    class Baker : Baker<CommuterAuthoring>
    {
        public override void Bake(CommuterAuthoring authoring)
        {
            var entity = GetEntity(authoring.CurrentPlatform);
            AddComponent<Commuter>(new Commuter()
            {
                Velocity = authoring.Velocity,
                CurrentPlatform = entity,
                Random = Unity.Mathematics.Random.CreateFromIndex((uint)GetEntity(authoring.gameObject).Index)
            }
            );

            AddComponent<TargetDestination>();
            AddComponent<SaschaMovementQueue>();
            AddComponent<QueueingData>();
            AddComponent<SeatReservation>();
        }
    }
}

struct Commuter : IComponentData
{
    public CommuterState State;
    public float Velocity;
    public Entity CurrentPlatform;
    public Unity.Mathematics.Random Random;
}

struct QueueingData : IComponentData
{
    public Entity TargetQueue;
    public int PositionInQueue;
}

struct SeatReservation : IComponentData
{
    public Entity TargetSeat;
}