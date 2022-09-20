using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// TODO - soon will be able to call Get/Set Component from IJE

#region step1
// This system should run after the transform system has been updated, otherwise the camera
// will lag one frame behind the tank and will jitter.
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial class CameraSystem : SystemBase
{
    Entity Target;
    Random Random;
    EntityQuery TanksQuery;

    protected override void OnCreate()
    {
        Random = Random.CreateFromIndex(1234);
        TanksQuery = GetEntityQuery(typeof(Tank));
        RequireForUpdate(TanksQuery);
    }

    protected override void OnUpdate()
    {
        if (Target == Entity.Null || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space))
        {
            var tanks = TanksQuery.ToEntityArray(Allocator.Temp);
            Target = tanks[Random.NextInt(tanks.Length)];
        }

        var cameraTransform = CameraSingleton.Instance.transform;
        var tankTransform = GetComponent<LocalToWorld>(Target);
        cameraTransform.position = tankTransform.Position - 10.0f * tankTransform.Forward + new float3(0.0f, 5.0f, 0.0f);
        cameraTransform.LookAt(tankTransform.Position, new float3(0.0f, 1.0f, 0.0f));
    }
}
#endregion