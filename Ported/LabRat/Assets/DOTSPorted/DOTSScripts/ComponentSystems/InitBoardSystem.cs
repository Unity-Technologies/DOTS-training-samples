using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;


// move to Tile component
// bitfield
// 0-3 bits indicate a wall
// 4 bit indicates a hole
public class TileMap : IComponentData
{
    public byte [] tiles;
}

public class InitBoardSystem : SystemBase
{
    // TODO: move to GameState/BoardInfo
    private readonly float4 color1 = new float4(0.95f, 0.95f, 0.95f, 1f);
    private readonly float4 color2 = new float4(0.80f, 0.78f, 0.88f, 1f);

    protected override void OnCreate()
    {
        UnityEngine.Debug.Log("InitBoardSystem OnCreate");
        base.OnCreate();
        RequireSingletonForUpdate<BoardSetup>();
        RequireSingletonForUpdate<BoardInfo>();
    }

    protected override void OnUpdate()
    {
        UnityEngine.Debug.Log("InitBoardSystem OnUpdate");

        BoardInfo boardInfo = GetSingleton<BoardInfo>();
        BoardSetup boardSetup = GetSingleton<BoardSetup>();
        int width = boardInfo.width;
        int height = boardInfo.height;

        int boardSize = width * height;
        byte [] tiles = new byte[boardSize];

        Entity tileEntity = EntityManager.CreateEntity(typeof(TileMap));
        EntityManager.SetComponentData(tileEntity, new TileMap { tiles = tiles });

        uint seed = (uint)DateTime.UtcNow.Millisecond + 1;
        Unity.Mathematics.Random random = new Unity.Mathematics.Random(seed);

        // TODO create tile Utils
        // set all wall edges
        for (int x = 0; x < width; x ++)
            for (int y = 0; y < height; y += height - 1)
            {
                int i = y*width + x;
                if (x == 0) tiles[i] |= (1 << 3);
                if (y == 0) tiles[i] |= (1 << 2);
                if (x == width-1) tiles[i] |= (1 << 1);
                if (y == height-1) tiles[i] |= (1 << 0);
            }        
        for (int x = 0; x < width; x += width - 1)
            for (int y = 0; y < height; y++)
            {
                int i = y*width + x;
                if (x == 0) tiles[i] |= (1 << 3);
                if (y == 0) tiles[i] |= (1 << 2);
                if (x == width-1) tiles[i] |= (1 << 1);
                if (y == height-1) tiles[i] |= (1 << 0);
            }

        // TODO: deterministic version
        // create tile data walls
        int numWalls = 0;
        int wallCount = boardInfo.numberOfWalls;
        while (numWalls < wallCount) {
            int x = random.NextInt(0, width-1);
            int y = random.NextInt(0, height-1);
            int w = random.NextInt(0, 3);
            int i = y*width + x;

            if ((tiles[i] & (1 << w)) == 0)
            {
                tiles[i] |= (byte)(1 << w);
                // set opposite tile wall bit
                if (w == 2) tiles[i - 1] |= (1 << 0);
                if (w == 0) tiles[i + 1] |= (1 << 2);
                if (w == 1) tiles[i + width] |= (1 << 3);
                if (w == 3) tiles[i - width] |= (1 << 1);
                numWalls++;
            }
        }

        // create holes/traps
        int holeCount = random.NextInt(
            boardInfo.minNumberOfHoles, boardInfo.maxNumberOfHoles);
        UnityEngine.Debug.Log("InitBoardSystem hole count min max, count: "
            + boardInfo.minNumberOfHoles + " " + boardInfo.maxNumberOfHoles + " "
            + holeCount);
        int numHoles = 0;
        while (numHoles < holeCount) {
            int x = random.NextInt(0, width-1);
            int y = random.NextInt(0, height-1);
            int i = y*width + x;

            if ((tiles[i] & (1 << 4)) == 0)
            {
                tiles[i] |= (1 << 4);
                numHoles++;
            }
        }

        // create goal prefabs
        float [] xoff = {0f, 0.5f, 0f, -0.5f};
        float [] yoff = {0.5f, 0f, -0.5f, 0f};
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
            {
                int i = y*width + x;
                // create tiles (check for holes)
                if ((tiles[i] & (1 << 4)) == 0)
                {
                    var cellPrefab = (x + y) % 2 == 0 ? boardSetup.cell0Prefab : boardSetup.cell1Prefab;
                    var cell = EntityManager.Instantiate(cellPrefab);
                    EntityManager.SetComponentData(cell, new Translation { Value = new float3(x, 0f, y) });
                }

                // create walls
                for(int w=0; w<4; w++)
                {
                    byte wallBit = (byte)(1 << w);
                    if ((tiles[i] & wallBit) != 0)// TODO: don't create duplicate walls on edges
                    {
                        var wallPrefab = boardSetup.wallPrefab;
                        var wall = EntityManager.Instantiate(wallPrefab);
                        if (((int)(w) % 2) == 1)
                        {
                            EntityManager.SetComponentData(wall, new Rotation { Value = quaternion.RotateY(0f) });
                        }
                        EntityManager.SetComponentData(wall, 
                            new Translation { Value = new float3(x + xoff[w], 0.78f, y + yoff[w]) });
                    }
                }
            }

        // TODO: create goal prefabs

        // TODO: move to GameState
        // colors for end spawn
        float4[] colors = {
            new float4(1.0f, 0.0f, 0.0f, 1.0f),
            new float4(0.0f, 1.0f, 0.0f, 1.0f),
            new float4(0.0f, 0.0f, 1.0f, 1.0f),
            new float4(0.0f, 0.0f, 0.0f, 1.0f) };

        // create mice and cat spawners

        // Remove BoardSetup Singleton to only run once
        EntityManager.RemoveComponent<BoardSetup>(GetSingletonEntity<BoardSetup>());
    }
}
