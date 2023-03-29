using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;

[BurstCompile]
public partial struct GridTilesSpawningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Let OnUpdate run for one frame
        state.Enabled = false;

        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var config = SystemAPI.GetSingleton<Config>();
        Random randomComponent = SystemAPI.GetSingleton<Random>();
        var fireNumber = config.startingFireCount;
        //var groundTiles = new NativeList<Entity>();

        for (int i = 0; i <= config.columns; i++)
        {
            for (int j = 0; j <= config.rows; j++)
            {
                var groundTile = ecb.Instantiate(config.Ground);
                
                ecb.SetComponent(groundTile, new LocalTransform
                {
                    Position = new float3
                    {
                        x = i * config.cellSize - 8f,
                        y = - (config.maxFlameHeight * 0.5f),
                        z = j * config.cellSize - 5f
                    },
                    Scale = 1,
                    Rotation = quaternion.identity
                });

                if (fireNumber > 0 && randomComponent.Value.NextInt(0, config.rows*config.columns) <= i+j) //temporary solution
                {
                    fireNumber--;
                    ecb.SetComponent(groundTile, new Tile { Temperature = randomComponent.Value.NextFloat(config.flashpoint, 1) });
                    ecb.SetComponentEnabled<OnFire>(groundTile,true);
                }
                else
                {
                    ecb.SetComponentEnabled<OnFire>(groundTile,false);
                }
                
                ecb.SetComponent(groundTile, new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)config.colour_fireCell_neutral });
                ecb.AddComponent(groundTile, new PostTransformScale { Value = float3x3.Scale(config.cellSize, config.maxFlameHeight, config.cellSize) });
                //groundTiles.Add(groundTile);
            }
        }
        
        /*while (fireNumber > 0)
        {
            var rand = randomComponent.Value.NextInt(0, config.columns+config.rows);
            if (!SystemAPI.IsComponentEnabled<OnFire>(groundTiles[rand])) // to avoid setting same tile
            {
                fireNumber--;
                ecb.SetComponent(groundTiles[rand], new Tile { Temperature = randomComponent.Value.NextFloat(config.flashpoint, 1) });
                ecb.SetComponentEnabled<OnFire>(groundTiles[rand],true);
            }
        }*/
    }
}
