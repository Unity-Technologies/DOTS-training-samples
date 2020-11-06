using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using ZombieMaze;

public class PlayerMovementSystem : SystemBase
{
    private CameraControl _cameraControl;
    private EntityQuery _playerPositionQuery;
    
    protected override void OnCreate()
    {
        _playerPositionQuery = EntityManager.CreateEntityQuery(ComponentType.ReadOnly<PlayerTag>(), 
            ComponentType.ReadOnly<Translation>());
    }

    protected override void OnUpdate()
    {
        if (_cameraControl == null)
        {
            _cameraControl = Camera.main.GetComponent<CameraControl>();
        }
        var positionDataArray = _playerPositionQuery.ToComponentDataArray<Translation>(Allocator.Temp);

        if (positionDataArray.Length > 0) 
        {
            _cameraControl.playerPosition = positionDataArray[0].Value;
        }

        var playerInput = GetSingleton<PlayerInput>();

        Entities.ForEach((ref Position position, ref Direction direction, in Speed speed, in PlayerTag _) =>
        {
            if (direction.MoveState == MoveState.IDLE)
            {
                if (math.abs(playerInput.Direction.x) > 0.5f)
                {
                    direction.Value.x = math.sign(playerInput.Direction.x);
                    direction.Value.y = 0f;
                }
                else if(math.abs(playerInput.Direction.y) > 0.5f)
                {
                    direction.Value.y = math.sign(playerInput.Direction.y);
                    direction.Value.x = 0f;
                }
                else
                {
                    direction.Value = new float2(0f);
                }
            }
        }).Schedule();
    }
}
