using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Baking;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(LateSimulationSystemGroup))]

partial struct CameraSystem : ISystem
{
    private EntityQuery TanksQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Player>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        var player = SystemAPI.GetSingletonEntity<Player>();

        var cameraTransfrom = CameraSingleton.Instance.transform;
        var playerTransform = SystemAPI.GetComponent<LocalToWorld>(player);

        cameraTransfrom.position =
            playerTransform.Position - 10.0f * playerTransform.Forward + new float3(0.0f, 5.0f, 0.0f);

        cameraTransfrom.LookAt(playerTransform.Position, new float3(0.0f, 1.0f, 0.0f));
    }
}