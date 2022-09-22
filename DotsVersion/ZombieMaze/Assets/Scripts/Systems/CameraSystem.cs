using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// This system should run after the transform system has been updated, otherwise the camera
// will lag one frame behind the tank and will jitter.
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial class CameraSystem : SystemBase
{
    Entity Target;
    Random Random;
    EntityQuery CharQuery;

    protected override void OnCreate()
    {
        Random = Random.CreateFromIndex(1234);
        CharQuery = GetEntityQuery(typeof(PlayerData));
        RequireForUpdate(CharQuery);
    }

    protected override void OnUpdate()
    {
        if (Target == Entity.Null || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space))
        {
            var tanks = CharQuery.ToEntityArray(Allocator.Temp);
            Target = tanks[Random.NextInt(tanks.Length)];
        }

        var cameraTransform = CameraSingleton.Instance.transform;
        var charTransform = GetComponent<LocalToWorld>(Target);
        cameraTransform.position = charTransform.Position - 50.0f * charTransform.Forward + new float3(0.0f, 20.0f, 0.0f);
        cameraTransform.LookAt(charTransform.Position, new float3(0.0f, 1.0f, 0.2f));
    }
}
