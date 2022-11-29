using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class PlatformAuthoring : MonoBehaviour
{
    public GameObject[] PlatformQueues;
    public GameObject LeftWalkway;
    public GameObject RightWalkway;
}

class PlatformBaker : Baker<PlatformAuthoring>
{
    public override void Bake(PlatformAuthoring authoring)
    {
        AddComponent(new Platform
        {
            // TODO - Hard coding the amount of queues? Do we need to initialize this data here?
            PlatformQueues = new NativeArray<Entity>(authoring.PlatformQueues.Length, Allocator.Persistent)
        });
    }
}