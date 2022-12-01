using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlatformAuthoring : MonoBehaviour
{
    public Transform[] PlatformQueues;
    public Transform WalkwayFrontLower;
    public Transform WalkwayFrontUpper;
    public Transform WalkwayBackLower;
    public Transform WalkwayBackUpper;
}

class PlatformBaker : Baker<PlatformAuthoring>
{
    public override void Bake(PlatformAuthoring authoring)
    {
        var queueCount = authoring.PlatformQueues.Length;
        var queuesPosRotations = new NativeArray<float3x2>(queueCount, Allocator.Persistent);
        for (int i = 0; i < queueCount; i++)
        {
            queuesPosRotations[i] = new float3x2(authoring.PlatformQueues[i].position, authoring.PlatformQueues[i].forward);
        }

        var position = authoring.transform.position;
        AddComponent(new Platform
        {
            PlatformQueues = queuesPosRotations,
            WalkwayFrontLower = authoring.WalkwayFrontLower.position - position,
            WalkwayFrontUpper = authoring.WalkwayFrontUpper.position - position,
            WalkwayBackLower = authoring.WalkwayBackLower.position - position,
            WalkwayBackUpper = authoring.WalkwayBackUpper.position - position
        });
        AddComponent<TrainOnPlatform>();
    }
}