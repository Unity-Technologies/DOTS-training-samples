using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
// This system should run after the transform system has been updated, otherwise the camera
// will lag one frame behind the tank and will jitter.
[UpdateInGroup(typeof(LateSimulationSystemGroup))]
partial struct CameraSystem : ISystem
{
    Entity Target;
    Unity.Mathematics.Random Random;
    EntityQuery FarmerQuery;

    float2 viewAngles;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    { 
        Random = Unity.Mathematics.Random.CreateFromIndex(1234);
        FarmerQuery = SystemAPI.QueryBuilder().WithAll<Farmer>().Build();
        state.RequireForUpdate(FarmerQuery);
        Cursor.lockState = CursorLockMode.Locked;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    // Because this OnUpdate accesses managed objects, it cannot be Burst compiled.
    public void OnUpdate(ref SystemState state)
    {
        //if (Target == Entity.Null || UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space))
        //{
        //    var farmers = FarmerQuery.ToEntityArray(Allocator.Temp);
        //    Target = farmers[Random.NextInt(farmers.Length)];
        //}

        //////handle mouse movement
        ////mouseInputDelta.x = Input.mousePosition.x - mouseInputLastFrame.x;
        ////mouseInputDelta.y = Input.mousePosition.y - mouseInputLastFrame.y;

        ////mouseInputLastFrame = (Vector2)Input.mousePosition;

        //var cameraTransform = CameraSingleton.Instance.transform;
        //var farmerTransform = SystemAPI.GetComponent<LocalTransform>(Target);
        ////cameraTransform.position = farmerTransform.Position - 10.0f * farmerTransform.Forward + new float3(0.0f, 5.0f, 0.0f);
        ////cameraTransform.RotateAround(farmerTransform.Position, Vector3.up * mouseInputDelta.x + Vector3.right * mouseInputDelta.y, (mouseInputDelta.x + mouseInputDelta.y));
        ////cameraTransform.LookAt(farmerTransform.Position, new float3(0.0f, 1.0f, 0.0f));

        //viewAngles.x += Input.GetAxis("Mouse X") * 4000 / Screen.height;
        //viewAngles.y -= Input.GetAxis("Mouse Y") * 4000 / Screen.height;
        //viewAngles.y = Mathf.Clamp(viewAngles.y, 7f, 80f);
        //viewAngles.x -= Mathf.Floor(viewAngles.x / 360f) * 360f;
        //cameraTransform.rotation = Quaternion.Euler(viewAngles.y, viewAngles.x, 0f);
        //cameraTransform.position = farmerTransform.Position - (float3)cameraTransform.forward * 10;


    }
}