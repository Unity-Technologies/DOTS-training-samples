using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct MovingWallCreationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PrefabConfig>();
        state.RequireForUpdate<MazeConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PrefabConfig prefabConfig = SystemAPI.GetSingleton<PrefabConfig>();
        MazeConfig mazeConfig = SystemAPI.GetSingleton<MazeConfig>();

        for (int i = 0; i < mazeConfig.MovingWallsToSpawn; ++i)
        {
            Entity wallEntity = state.EntityManager.Instantiate(prefabConfig.MovingWallPrefab);
            TransformAspect transformAspect = SystemAPI.GetAspectRW<TransformAspect>(wallEntity);
            
            Vector2Int randomTile = mazeConfig.GetRandomTilePositionInside(mazeConfig.MovingWallSize, 1);
            transformAspect.Position += new float3(randomTile.x, 0.0f, randomTile.y);

            PostTransformMatrix postTransformMatrix = SystemAPI.GetComponent<PostTransformMatrix>(wallEntity);
            postTransformMatrix.Value = float4x4.Scale(mazeConfig.MovingWallSize, postTransformMatrix.Value.c1.y, postTransformMatrix.Value.c2.z);
            SystemAPI.SetComponent<PostTransformMatrix>(wallEntity, postTransformMatrix);

            MovingWall movingWall = SystemAPI.GetComponent<MovingWall>(wallEntity);
            movingWall.MoveSpeedInSeconds = UnityEngine.Random.Range(mazeConfig.MovingWallMinMoveSpeedInSeconds, mazeConfig.MovingWallMaxMoveSpeedInSeconds);
            movingWall.NumberOfTilesToMove = UnityEngine.Random.Range(mazeConfig.MovingWallMinTilesToMove, mazeConfig.MovingWallMaxTilesToMove);
            movingWall.StartXIndex = randomTile.x;
            movingWall.CurrentXIndex = randomTile.x;
            SystemAPI.SetComponent<MovingWall>(wallEntity, movingWall);
        }

        state.Enabled = false;
    }
}
