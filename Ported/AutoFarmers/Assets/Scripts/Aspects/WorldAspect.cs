using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

readonly partial struct WorldAspect : IAspect
{
    public readonly Entity Self;

    public readonly RefRW<WorldGrid> Grid;
    public readonly DynamicBuffer<ChunkCell> Chunks;

    public void Initialize(ref SystemState state)
    {
        int width = Grid.ValueRW.gridSize.x;
        int height = Grid.ValueRW.gridSize.y;
        Grid.ValueRW.typeGrid = new NativeArray<byte>(width * height, Allocator.Persistent);
        Grid.ValueRW.entityGrid = new NativeArray<Entity>(width * height, Allocator.Persistent);

        //Let's make the chunks first
        DynamicBuffer<ChunkCell> chunkBuffer = state.EntityManager.GetBuffer<ChunkCell>(Grid.ValueRW.entity);
        chunkBuffer.Length = (width * height) / ChunkCell.size;

        for (int i = 0; i < chunkBuffer.Length; i++)
        {
            var chunkcell = chunkBuffer[i];
            chunkcell.typeCount = new NativeArray<int>(ChunkCell.maxTypes, Allocator.Persistent);
            chunkBuffer[i] = chunkcell;
        }

        Grid.ValueRW.initialized = true;
    }

    public int Width
    {
        get => Grid.ValueRW.gridSize.x;
    }

    public int Height
    {
        get => Grid.ValueRW.gridSize.y;
    }

    #region Grid Interfacing
    public float3 GridToWorld(int x, int y)
    {
        return Grid.ValueRO.GridToWorld(x, y);
    }

    public float3 GridToWorld(int2 pos)
    {
        return Grid.ValueRO.GridToWorld(pos);
    }

    public int2 WorldToGrid(float3 pos)
    {
        return Grid.ValueRO.WorldToGrid(pos);
    }

    public int GetTypeAt(int x, int y)
    {
        return Grid.ValueRO.GetTypeAt(x, y);
    }

    public int GetTypeAt(int2 point)
    {
        return Grid.ValueRO.GetTypeAt(point);
    }

    public void SetEntityAt(int x, int y, Entity e)
    {
        Grid.ValueRW.SetEntityAt(x, y, e);
    }

    public void SetEntityAt(int2 point, Entity e)
    {
        Grid.ValueRW.SetEntityAt(point, e);
    }

    public Entity GetEntityAt(int x, int y)
    {
        return Grid.ValueRO.GetEntityAt(x, y);
    }

    public Entity GetEntityAt(int2 point)
    {
        return Grid.ValueRO.GetEntityAt(point);
    }

    public void SetTypeAt(int x, int y, byte type)
    {
        Grid.ValueRW.SetTypeAt(x,y,type);
    }

    public void SetTypeAt(int2 point, byte type)
    {
        Grid.ValueRW.SetTypeAt(point, type);
    }
    #endregion

}
