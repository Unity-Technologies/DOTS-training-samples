using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
partial struct MovementSystem : ISystem
{
    ComponentLookup<HasReachedDestinationTag> m_HasReachedDestinationTagLookup;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_HasReachedDestinationTagLookup = state.GetComponentLookup<HasReachedDestinationTag>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        m_HasReachedDestinationTagLookup.Update(ref state);

        var moveWorkerJob = new MoveWorkerJob()
        {
            HasReachedDestinationTagLookup = m_HasReachedDestinationTagLookup,
            deltaTime = SystemAPI.Time.DeltaTime
        };
        moveWorkerJob.Schedule();
    }
}

[WithNone(typeof(HasReachedDestinationTag))]
[BurstCompile]
partial struct MoveWorkerJob : IJobEntity
{
    public ComponentLookup<HasReachedDestinationTag> HasReachedDestinationTagLookup;
    public float deltaTime;

    void Execute(in Entity entity, ref TransformAspect transform, ref Position position, in MoveInfo moveInfo)
    {
        var direction = moveInfo.destinationPosition - position.position;
        var distanceToDestination = math.length(direction);
        var distanceToTravel = deltaTime * moveInfo.speed;
        if (distanceToTravel > distanceToDestination)
        {
            position.position = moveInfo.destinationPosition;
            HasReachedDestinationTagLookup.SetComponentEnabled(entity, true);
        }
        else
        {
            position.position += distanceToTravel * direction / distanceToDestination;
        }
        
        transform.LocalPosition = new float3(position.position.x, transform.LocalPosition.y, position.position.y);
    }
}


