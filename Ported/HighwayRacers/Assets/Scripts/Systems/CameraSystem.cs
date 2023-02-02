using Unity.Collections;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
// This system should run after the transform system has been updated, otherwise the camera
// will lag one frame behind the Car and will jitter.
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct CameraSystem : ISystem
{
    Entity Target;
    Random Random;
    EntityQuery CarsQuery;
    private float CameraHeight;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Random = Random.CreateFromIndex(1234);
        CarsQuery = SystemAPI.QueryBuilder().WithAll<CarPositionInLane>().Build(); 
        state.RequireForUpdate(CarsQuery);

        CameraHeight = 5.0f;
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


        var carTransform = SystemAPI.GetComponent<LocalToWorld>(Target);
        
        float camViewChangeAlpha = math.clamp((CameraHeight - 50.0f) / 1000.0f, 0,1);
        CameraHeight = math.clamp(CameraHeight + (UnityEngine.Input.mouseScrollDelta.y * (1+camViewChangeAlpha*3)),5.0f, 6000);
        camViewChangeAlpha = math.clamp((CameraHeight - 50.0f) / 1000.0f, 0,1);

        var cameraTransform = CameraSingleton.Instance.transform;
        
        cameraTransform.position = math.lerp(
            (carTransform.Position - 10.0f * carTransform.Forward + new float3(0.0f, CameraHeight, 0.0f)),
            new float3(0,CameraHeight,0), camViewChangeAlpha);
        
        cameraTransform.LookAt(math.lerp(carTransform.Position, float3.zero, camViewChangeAlpha), new float3(0.0f, 1.0f, 0.0f));
    }
}