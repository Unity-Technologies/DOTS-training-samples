using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

public struct PlayerInput : IComponentData
{
    public float2 Direction;
    public float  ZoomDelta;
}

public class PlayerInputSystem : SystemBase
{
    protected override void OnCreate()
    {
        var e = EntityManager.CreateEntity(typeof(PlayerInput));
#if UNITY_EDITOR
        EntityManager.SetName(e, "PlayerInput");
#endif
    }

    protected override void OnUpdate()
    {
        var playerInputEntity = GetSingletonEntity<PlayerInput>();
        PlayerInput playerInput;
        playerInput.Direction = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        playerInput.ZoomDelta = Input.mouseScrollDelta.y;
        EntityManager.SetComponentData(playerInputEntity, playerInput);
    }
}
