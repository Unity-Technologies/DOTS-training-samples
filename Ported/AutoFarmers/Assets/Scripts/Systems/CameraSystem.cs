using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
// This system should run after the transform system has been updated, otherwise the camera
// will lag one frame behind the tank and will jitter.
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct CameraSystem : ISystem
{
    Entity Target;
    Random Random;
    EntityQuery FarmerQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Random = Random.CreateFromIndex(1234);
        FarmerQuery = SystemAPI.QueryBuilder().WithAll<Farmer>().Build();
        state.RequireForUpdate(FarmerQuery);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    // Because this OnUpdate accesses managed objects, it cannot be Burst compiled.
    public void OnUpdate(ref SystemState state)
    {
        if (Target == Entity.Null || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space))
        {
            var farmers = FarmerQuery.ToEntityArray(Allocator.Temp);
            Target = farmers[Random.NextInt(farmers.Length)];
        }

        var cameraTransform = CameraSingleton.Instance.transform;
        var farmerTransform = SystemAPI.GetComponent<LocalToWorld>(Target);
        cameraTransform.position = farmerTransform.Position - 10.0f * farmerTransform.Forward + new float3(0.0f, 5.0f, 0.0f);
        cameraTransform.LookAt(farmerTransform.Position, new float3(0.0f, 1.0f, 0.0f));
    }
}