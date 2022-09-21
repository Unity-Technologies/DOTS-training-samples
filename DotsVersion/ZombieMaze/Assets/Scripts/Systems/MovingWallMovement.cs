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
        state.RequireForUpdate<TileBufferElement>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        MazeConfig mazeConfig = SystemAPI.GetSingleton<MazeConfig>();
        DynamicBuffer<TileBufferElement> tiles = SystemAPI.GetSingletonBuffer<TileBufferElement>();

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

                    int rightEndIndex = movingWall.CurrentXIndex + mazeConfig.MovingWallSize / 2;
                    int leftEndIndex = movingWall.CurrentXIndex - mazeConfig.MovingWallSize / 2;

                    TileBufferElement upTile = tiles[mazeConfig.Get1DIndex(leftEndIndex, movingWall.StartYIndex + 1)];
                    upTile.DownWall = true;
                    tiles[mazeConfig.Get1DIndex(leftEndIndex, movingWall.StartYIndex + 1)] = upTile;

                    TileBufferElement downTile = tiles[mazeConfig.Get1DIndex(leftEndIndex, movingWall.StartYIndex)];
                    downTile.UpWall = true;
                    tiles[mazeConfig.Get1DIndex(leftEndIndex, movingWall.StartYIndex)] = downTile;

                    upTile = tiles[mazeConfig.Get1DIndex(rightEndIndex, movingWall.StartYIndex + 1)];
                    upTile.DownWall = false;
                    tiles[mazeConfig.Get1DIndex(rightEndIndex, movingWall.StartYIndex + 1)] = upTile;

                    downTile = tiles[mazeConfig.Get1DIndex(rightEndIndex, movingWall.StartYIndex)];
                    downTile.UpWall = false;
                    tiles[mazeConfig.Get1DIndex(rightEndIndex, movingWall.StartYIndex)] = downTile;
                }
                else
                {
                    movingWall.CurrentXIndex++;
                    movingWall.Position += new float3(1.0f, 0.0f, 0.0f);

                    int rightEndIndex = movingWall.CurrentXIndex + mazeConfig.MovingWallSize / 2;
                    int leftEndIndex = movingWall.CurrentXIndex - mazeConfig.MovingWallSize / 2;

                    TileBufferElement upTile = tiles[mazeConfig.Get1DIndex(leftEndIndex, movingWall.StartYIndex + 1)];
                    upTile.DownWall = false;
                    tiles[mazeConfig.Get1DIndex(leftEndIndex, movingWall.StartYIndex + 1)] = upTile;

                    TileBufferElement downTile = tiles[mazeConfig.Get1DIndex(leftEndIndex, movingWall.StartYIndex)];
                    downTile.UpWall = false;
                    tiles[mazeConfig.Get1DIndex(leftEndIndex, movingWall.StartYIndex)] = downTile;


                    upTile = tiles[mazeConfig.Get1DIndex(rightEndIndex, movingWall.StartYIndex + 1)];
                    upTile.DownWall = true;
                    tiles[mazeConfig.Get1DIndex(rightEndIndex, movingWall.StartYIndex + 1)] = upTile;

                    downTile = tiles[mazeConfig.Get1DIndex(rightEndIndex, movingWall.StartYIndex)];
                    downTile.UpWall = true;
                    tiles[mazeConfig.Get1DIndex(rightEndIndex, movingWall.StartYIndex)] = downTile;
                }

                if(Mathf.Abs(movingWall.CurrentXIndex - movingWall.StartXIndex) >= movingWall.NumberOfTilesToMove)
                {
                    movingWall.MovingLeft = !movingWall.MovingLeft;
                }
            }
        }
    }
}
