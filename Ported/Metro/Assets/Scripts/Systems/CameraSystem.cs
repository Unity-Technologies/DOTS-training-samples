using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

// This system should run after the transform system has been updated, otherwise the camera
// will lag one frame behind the tank and will jitter.
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial class CameraSystem : SystemBase
{
    Entity Target;
    Random Random;
    EntityQuery PlatformQuery;
    EntityQuery TrainQuery;
    float distance;
    float3 offset;

    protected override void OnCreate()
    {
        Random = Random.CreateFromIndex(1234);
        PlatformQuery = GetEntityQuery(typeof(Platform));
        TrainQuery = GetEntityQuery(typeof(Train));
        RequireForUpdate(PlatformQuery);
    }

    protected override void OnUpdate()
    {
        if (Target == Entity.Null || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.P))
        {
            var platforms = PlatformQuery.ToEntityArray(Allocator.Temp);
            Target = platforms[Random.NextInt(platforms.Length)];
            distance = 20;
            offset = new float3(10.0f, 20.0f, 0.0f);
        } else if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.T))
        {
            var trains = TrainQuery.ToEntityArray(Allocator.Temp);
            Target = trains[Random.NextInt(trains.Length)];
            distance = 20;
            offset = new float3(0.0f, 5.0f, 0.0f);
        }
        
        var cameraTransform = MetroCamera.Instance.transform;
        var targetTransform = GetComponent<LocalToWorld>(Target);
        cameraTransform.position = targetTransform.Position - distance * targetTransform.Forward + offset;
        cameraTransform.LookAt(targetTransform.Position, new float3(0.0f, 1.0f, 0.0f));
    }
}