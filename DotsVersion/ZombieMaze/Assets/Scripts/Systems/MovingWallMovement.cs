using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

[BurstCompile]
public partial struct MovingWallMovement : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MazeConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        MazeConfig mazeConfig = SystemAPI.GetSingleton<MazeConfig>();
        float deltaTime = state.Time.DeltaTime;

        foreach (var movingWall in SystemAPI.Query<MovingWallAspect>())
        {
            movingWall.MoveTimer += deltaTime;
            if (movingWall.MoveTimer >= movingWall.MoveSpeedInSeconds)
            {
                movingWall.MoveTimer -= movingWall.MoveSpeedInSeconds;
                if(movingWall.MovingLeft)
                {
                    movingWall.CurrentXIndex--;
                    movingWall.Position -= new float3(1.0f, 0.0f, 0.0f);
                }
                else
                {
                    movingWall.CurrentXIndex++;
                    movingWall.Position += new float3(1.0f, 0.0f, 0.0f);
                }

                if(Mathf.Abs(movingWall.CurrentXIndex - movingWall.StartXIndex) >= movingWall.NumberOfTilesToMove)
                {
                    movingWall.MovingLeft = !movingWall.MovingLeft;
                }
            }
        }
    }
}
