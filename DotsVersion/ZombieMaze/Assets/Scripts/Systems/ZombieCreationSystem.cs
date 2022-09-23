using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = System.Random;

[BurstCompile]
public partial struct ZombieCreationSystem : ISystem
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
        var Random = Unity.Mathematics.Random.CreateFromIndex(1234);
        int width = mazeConfig.Width - 1;
        int height = mazeConfig.Height - 1;
        int zombiesCount = mazeConfig.ZombiesToSpawn;

        int counter = 0;

        while (counter < zombiesCount)
        {
            counter++;
            var random = new Random();
            var side = random.Next(0, 4);
            int x = 0, y = 0;

            switch (side)
            {
                case 0: //Left
                    y = random.Next(0, height);
                    x = 0;
                    break;
                case 1: //Right
                    y = random.Next(0, height);
                    x = width;
                    break;
                case 2: //Top
                    x = random.Next(0, width);
                    y = height;
                    break;
                case 3: //Bottom
                    x = random.Next(0, width);
                    y = 0;
                    break;
            }

            int zombieIndex = random.Next(0, 3);
            switch (zombieIndex)
            {
                case 0:
                {
                    Entity zombie = state.EntityManager.Instantiate(prefabConfig.ZombiePrefab);
                    TransformAspect wallTransform = SystemAPI.GetAspectRW<TransformAspect>(zombie);
                    wallTransform.LocalPosition = new Vector3(x, wallTransform.LocalPosition.y - 1, y);

                    Target target = new Target()
                    {
                        TargetPosition = new int2(x, y),
                        PathIndex = -1
                    };
                    SystemAPI.SetComponent(zombie, target);
                    break;
                }
                case 1:
                {
                    Entity zombie = state.EntityManager.Instantiate(prefabConfig.ZombieChaserPrefab);
                    TransformAspect wallTransform = SystemAPI.GetAspectRW<TransformAspect>(zombie);
                    wallTransform.LocalPosition = new Vector3(x, wallTransform.LocalPosition.y - 1, y);

                    Target target = new Target()
                    {
                        TargetPlayer = true,
                        TargetPosition = new int2(x, y),
                        PathIndex = -1
                    };
                    SystemAPI.SetComponent(zombie, target);
                    break;
                }
                case 2:
                {
                    Entity zombie = state.EntityManager.Instantiate(prefabConfig.ZombieRandomPrefab);
                    TransformAspect wallTransform = SystemAPI.GetAspectRW<TransformAspect>(zombie);
                    wallTransform.LocalPosition = new Vector3(x, wallTransform.LocalPosition.y - 1, y);

                    RandomMovement randomMovement = new RandomMovement() { TargetPosition = new int2(x, y) };
                    SystemAPI.SetComponent(zombie, randomMovement);
                    break;
                }
            }

            mazeConfig.ZombiesToSpawn = 0;
            SystemAPI.SetSingleton<MazeConfig>(mazeConfig);
        }
    }
}