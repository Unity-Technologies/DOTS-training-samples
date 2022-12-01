using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class PassengerAuthoring : MonoBehaviour
{
}

class PassengerBaker : Baker<PassengerAuthoring>
{
    public override void Bake(PassengerAuthoring authoring)
    {
        AddComponent(new Passenger { State = PassengerState.Idle });
        AddComponent(new PlatformId());
        AddComponent(new PlatformQueueId());
        AddBuffer<Waypoint>();
        var waypoints = AddBuffer<Waypoint>();
    }
}


// TODO - Move this to another file
[InternalBufferCapacity(4)]
public struct Waypoint : IBufferElementData, IEnableableComponent
{
    public float3 Value;
}

