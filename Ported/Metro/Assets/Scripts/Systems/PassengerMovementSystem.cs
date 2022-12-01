using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Systems;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
[WithAll(typeof(Passenger))]
partial struct PassengerMovementJob : IJobEntity
{
    //public Entity PassengerEntity;
    [NativeDisableParallelForRestriction] public BufferLookup<Waypoint> WaypointLookup;
    public float Speed;
    public float DeltaTime;
    
    void Execute(ref PassengerAspect passenger)
    {
        if (!WaypointLookup.IsBufferEnabled(passenger.Self))
            return;
        if (passenger.State == PassengerState.Idle || passenger.State == PassengerState.InQueue)
            return;
        DynamicBuffer<Waypoint> waypoints;
        bool success = WaypointLookup.TryGetBuffer(passenger.Self, out waypoints);
        if ((waypoints.Length == 0)||!success)
        {
            WaypointLookup.SetBufferEnabled(passenger.Self, false);
            return;
        }
        float3 destination = waypoints[0].Value;
        var direction = destination - passenger.Position;
        var distance = math.lengthsq(direction);
        if (distance > 0.001f)
        {
            passenger.Position += math.normalize(direction) * (DeltaTime * Speed);
        }
        else
        {
            waypoints.RemoveAt(0);
            passenger.State = PassengerState.Idle;
        }
    }
}

[BurstCompile]
[RequireMatchingQueriesForUpdate]
partial struct PassengerMovementSystem : ISystem
{
    BufferLookup<Waypoint> m_WaypointLookup;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        m_WaypointLookup = state.GetBufferLookup<Waypoint>();
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
            Speed = 4,
            DeltaTime = dt
        };
        movementJob.ScheduleParallel();
    }
}
