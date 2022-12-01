using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
[WithAll(typeof(Passenger))]
partial struct PassengerMovementJob : IJobEntity
{
    //public Entity PassengerEntity;
    [NativeDisableParallelForRestriction] public BufferLookup<Waypoint> WaypointLookup;
    public float DeltaTime;

    void Execute(ref PassengerAspect passenger)
    {
        if (!WaypointLookup.IsBufferEnabled(passenger.Self))
            return;
        if (passenger.State == PassengerState.Idle || passenger.State == PassengerState.InQueue || passenger.State == PassengerState.Seated)
            return;
        WaypointLookup.TryGetBuffer(passenger.Self, out var waypoints);
        if (waypoints.Length == 0)
        {
            if (passenger.State == PassengerState.OffBoarding)
                passenger.State = PassengerState.Idle;
            else
                passenger.State++;
            if (passenger.State != PassengerState.InQueue && passenger.State != PassengerState.Seated)
                WaypointLookup.SetBufferEnabled(passenger.Self, false);
            return;
        }

        float3 destination = waypoints[0].Value;
        var direction = destination - passenger.Position;
        var distance = math.lengthsq(direction);
        if (distance > 0.001f)
        {
            var nextSuggestedPosition = passenger.Position + math.normalize(direction) * (DeltaTime * passenger.Speed);
            var distanceToNextPosition = math.distancesq(passenger.Position, nextSuggestedPosition);
            if (distanceToNextPosition < distance)
                passenger.Position = nextSuggestedPosition;
            else
                passenger.Position = destination;
        }
        else
            waypoints.RemoveAt(0);
    }
}

[BurstCompile]
[RequireMatchingQueriesForUpdate]
partial struct PassengerMovementSystem : ISystem
{
    BufferLookup<Waypoint> m_WaypointLookup;
    ComponentLookup<SpeedComponent> m_SpeedLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_WaypointLookup = state.GetBufferLookup<Waypoint>();
        m_SpeedLookup = state.GetComponentLookup<SpeedComponent>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Random random = new Random(1234);
        // foreach ((var passenger, var entity) in SystemAPI.Query<Passenger>().WithEntityAccess())
        // {
        //     var transform = SystemAPI.GetComponent<LocalTransform>(entity);
        //     if (passenger.State == PassengerState.Waiting)
        //     {
        //         SystemAPI.SetComponent<Passenger>(entity, new Passenger()
        //         {
        //             State = PassengerState.Walking,
        //             Destination = new float3(transform.Position.x + random.NextFloat(-5,5),0,transform.Position.z + random.NextFloat(-5,5))
        //         });
        //     }
        //     else if (passenger.State == PassengerState.Walking)
        //     {
        //         float3 dir = passenger.Destination - transform.Position;
        //         var distanceToThePoint = math.lengthsq(dir);
        //         if (distanceToThePoint > 0.001f) 
        //         {
        //             var pos = transform.Position + math.normalize(dir) * 3 * SystemAPI.Time.DeltaTime;
        //             SystemAPI.SetComponent<LocalTransform>(entity, LocalTransform.FromPosition(pos));
        //         }
        //         else
        //         {
        //             SystemAPI.SetComponent<Passenger>(entity, new Passenger()
        //             {
        //                 State = PassengerState.Waiting
        //             });
        //         }
        //     }
        // }

        m_WaypointLookup.Update(ref state);
        float dt = SystemAPI.Time.DeltaTime;
        var movementJob = new PassengerMovementJob
        {
            WaypointLookup = m_WaypointLookup,
            DeltaTime = dt
        };
        movementJob.ScheduleParallel();
    }
}