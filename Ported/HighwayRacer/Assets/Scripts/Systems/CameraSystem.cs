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
    EntityQuery CarsQuery;
    private float scrollAmount;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Random = Random.CreateFromIndex(1234);
        CarsQuery = SystemAPI.QueryBuilder().WithAll<Car>().Build();
        state.RequireForUpdate(CarsQuery);
        scrollAmount = 1f;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    // Because this OnUpdate accesses managed objects, it cannot be Burst-compiled.
    public void OnUpdate(ref SystemState state)
    {
        if (Target == Entity.Null || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space))
        {
            var cars = CarsQuery.ToEntityArray(Allocator.Temp);
            Target = cars[Random.NextInt(cars.Length)];
        }

        var cameraTransform = CameraSingleton.Instance.transform;
        var carTransform = SystemAPI.GetComponent<LocalToWorld>(Target);

        if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftAlt))
        {
            scrollAmount += UnityEngine.Input.mouseScrollDelta.y * 0.25f;
            cameraTransform.position = carTransform.Position + 20.0f * scrollAmount * carTransform.Up + new float3(0.0f, 5.0f, 0.0f);
        }
        else
            cameraTransform.position = carTransform.Position - 10.0f * carTransform.Forward + new float3(0.0f, 5.0f, 0.0f);

        cameraTransform.LookAt(carTransform.Position, new float3(0.0f, 1.0f, 0.0f));
    }
}
