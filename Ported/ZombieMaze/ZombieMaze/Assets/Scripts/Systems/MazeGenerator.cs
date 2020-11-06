using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Tilemaps;

public static class MapUtil
{
    public static void SetTile(ref DynamicBuffer<MapCell> buffer, MapCell mapCell, int2 coordinates, int width)
    {
        buffer[coordinates.x + coordinates.y * width] = mapCell;
    }

    public static MapCell GetTile(in DynamicBuffer<MapCell> buffer, int2 coordinates, int width)
    {
        return buffer[coordinates.x + coordinates.y * width];
    }

    public static bool IsVisited(MapCell mapCell)
    {
        return (mapCell.Value & (byte) WallBits.Visited) != 0;
    }

    public static void Visit(ref DynamicBuffer<MapCell> buffer, int2 coordinates, int width)
    {
        var mapCell = GetTile(in buffer, coordinates, width);
        mapCell.Value |= (byte) WallBits.Visited;
        SetTile(ref buffer, mapCell, coordinates, width);
    }

    public static void ClearVisited(ref DynamicBuffer<MapCell> buffer, int2 coordinates, int width)
    {
        var mapCell = GetTile(in buffer, coordinates, width);
        mapCell.Value &= (byte) ~WallBits.Visited;
        SetTile(ref buffer, mapCell, coordinates, width);
    }

    public static void SetWall(ref DynamicBuffer<MapCell> buffer, WallBits wall, int2 coordinates, int width)
    {
        var mapCell = GetTile(in buffer, coordinates, width);
        mapCell.Value |= (byte) wall;
        SetTile(ref buffer, mapCell, coordinates, width);
    }

    public static void ClearWall(ref DynamicBuffer<MapCell> buffer, WallBits wall, int2 coordinates, int width)
    {
        var mapCell = GetTile(in buffer, coordinates, width);
        mapCell.Value &= (byte) ~wall;
        SetTile(ref buffer, mapCell, coordinates, width);
    }
}

public class MazeGenerator : SystemBase
{
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
    private EntityQuery _mazeSpawnerQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        _mazeSpawnerQuery = GetEntityQuery(ComponentType.ReadOnly<Spawner>(),
            ComponentType.ReadOnly<MazeSpawner>());
    }

    private static void spawnLeftWall(Entity prefab, int2 position, int2 halfSize,
        EntityCommandBuffer.ParallelWriter ecb, int sortKey)
    {
        var spawnedTile = ecb.Instantiate(sortKey, prefab);
        ecb.SetComponent(sortKey, spawnedTile,
            new Translation {Value = new float3(position.x - halfSize.x - 0.5f, 0, position.y - halfSize.y)});
        ecb.SetComponent(sortKey, spawnedTile, new Rotation {Value = quaternion.Euler(0, math.PI / 2, 0)});
    }

    private static void spawnBottomWall(Entity prefab, int2 position, int2 halfSize,
        EntityCommandBuffer.ParallelWriter ecb, int sortKey)
    {
        var spawnedTile = ecb.Instantiate(sortKey, prefab);
        ecb.SetComponent(sortKey, spawnedTile,
            new Translation {Value = new float3(position.x - halfSize.x, 0, position.y - halfSize.y - 0.5f)});
    }

    private static void spawnTopWall(Entity prefab, int2 position, int2 halfSize,
        EntityCommandBuffer.ParallelWriter ecb, int sortKey)
    {
        var spawnedTile = ecb.Instantiate(sortKey, prefab);
        ecb.SetComponent(sortKey, spawnedTile,
            new Translation {Value = new float3(position.x - halfSize.x, 0, position.y - halfSize.y + 0.5f)});
    }
    
    private static void spawnRightWall(Entity prefab, int2 position, int2 halfSize,
        EntityCommandBuffer.ParallelWriter ecb, int sortKey)
    {
        var spawnedTile = ecb.Instantiate(sortKey, prefab);
        ecb.SetComponent(sortKey, spawnedTile,
            new Translation {Value = new float3(position.x - halfSize.x + 0.5f, 0, position.y - halfSize.y)});
        ecb.SetComponent(sortKey, spawnedTile, new Rotation {Value = quaternion.Euler(0, math.PI / 2, 0)});
    }

    struct MazeSpawnJob : IJobParallelFor
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        [ReadOnly]
        public int2 MazeSize;
        [ReadOnly]
        public Entity Prefab;
        [ReadOnly]
        public DynamicBuffer<MapCell> Tiles;

        public void Execute(int index)
        {
            int2 halfSize = MazeSize / 2;

            int i = index % MazeSize.x;
            int j = index / MazeSize.y;

                var position = new int2(i, j);
                var mapCell = MapUtil.GetTile(in Tiles, position, MazeSize.x);
                if ((mapCell.Value & (byte) WallBits.Bottom) != 0)
                    spawnBottomWall(Prefab, position, halfSize, ecb, index);
                if ((mapCell.Value & (byte) WallBits.Top) != 0)
                    spawnTopWall(Prefab, position, halfSize, ecb, index);
                if ((mapCell.Value & (byte) WallBits.Right) != 0)
                    spawnRightWall(Prefab, position, halfSize, ecb, index);
                if ((mapCell.Value & (byte) WallBits.Left) != 0)
                    spawnLeftWall(Prefab, position, halfSize, ecb, index);
        }
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        var parallelEcb = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        var stack = new NativeList<int2>(Allocator.TempJob);
        var unvisitedNeighbors = new NativeList<int2>(Allocator.TempJob);
        
        var mazeJobHandle = Entities.WithDisposeOnCompletion(stack).WithDisposeOnCompletion(unvisitedNeighbors).ForEach(
            (ref DynamicBuffer<MapCell> tiles, ref Random random, in MazeSpawner mazeSpawner, in Spawner spawner) =>
            {
                // generating maze with recursive backtracker algorithm - https://en.wikipedia.org/wiki/Maze_generation_algorithm
                int width = spawner.MazeSize.x;
                int length = spawner.MazeSize.y;
                var numTiles = width * length;
                int2 current = random.Value.NextInt2(new int2(0, width), new int2(0, length));

                MapUtil.Visit(ref tiles, current, width);
                int numVisited = 1;

                while (numVisited < numTiles)
                {
                    // choose random adjacent unvisited tile
                    unvisitedNeighbors.Clear();
                    if (current.x > 0 && !MapUtil.IsVisited(
                        MapUtil.GetTile(in tiles, new int2(current.x - 1, current.y), width)))
                        unvisitedNeighbors.Add(new int2(current.x - 1, current.y));
                    if (current.y > 0 && !MapUtil.IsVisited(
                        MapUtil.GetTile(in tiles, new int2(current.x, current.y - 1), width)))
                        unvisitedNeighbors.Add(new int2(current.x, current.y - 1));
                    if (current.x < width - 1 && !MapUtil.IsVisited(
                        MapUtil.GetTile(in tiles, new int2(current.x + 1, current.y), width)))
                        unvisitedNeighbors.Add(new int2(current.x + 1, current.y));
                    if (current.y < length - 1 && !MapUtil.IsVisited(
                        MapUtil.GetTile(in tiles, new int2(current.x, current.y + 1), width)))
                        unvisitedNeighbors.Add(new int2(current.x, current.y + 1));

                    if (unvisitedNeighbors.Length > 0)
                    {
                        // visit neighbor
                        int2 next = unvisitedNeighbors[random.Value.NextInt(0, unvisitedNeighbors.Length)];
                        stack.Add(current);
                        // remove wall between tiles
                        if (next.x > current.x)
                        {
                            MapUtil.ClearWall(ref tiles, WallBits.Right, current, width);
                            MapUtil.ClearWall(ref tiles, WallBits.Left, next, width);
                        }
                        else if (next.y > current.y)
                        {
                            MapUtil.ClearWall(ref tiles, WallBits.Top, current, width);
                            MapUtil.ClearWall(ref tiles, WallBits.Bottom, next, width);
                        }
                        else if (next.x < current.x)
                        {
                            MapUtil.ClearWall(ref tiles, WallBits.Left, current, width);
                            MapUtil.ClearWall(ref tiles, WallBits.Right, next, width);
                        }
                        else if (next.y < current.y)
                        {
                            MapUtil.ClearWall(ref tiles, WallBits.Bottom, current, width);
                            MapUtil.ClearWall(ref tiles, WallBits.Top, next, width);
                        }

                        MapUtil.Visit(ref tiles, next, width);
                        numVisited++;
                        current = next;
                    }
                    else
                    {
                        // backtrack if no unvisited neighboring tiles
                        if (stack.Length > 0)
                        {
                            current = stack[stack.Length - 1];
                            stack.RemoveAtSwapBack(stack.Length - 1);
                            ;
                        }
                    }
                }

                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (j % (mazeSpawner.MazeStripsWidth + mazeSpawner.OpenStripsWidth) >=
                            mazeSpawner.OpenStripsWidth)
                            continue;
                        
                        WallBits bits = WallBits.Bottom | WallBits.Left | WallBits.Right | WallBits.Top;

                        if (i == 0)
                            bits &= ~WallBits.Bottom;
                        else if (i == length - 1)
                            bits &= ~WallBits.Top;
                            
                        if (j == 0)
                            bits &= ~WallBits.Left;
                        else if (j == width - 1)
                            bits &= ~WallBits.Right;
                            
                        MapUtil.ClearWall(ref tiles, bits,new int2(j, i), width);
                    }
                }
            }).Schedule(Dependency);

        Dependency = JobHandle.CombineDependencies(mazeJobHandle, Dependency);

        //Complete maze job to read back cells
        mazeJobHandle.Complete();
        
        var spawnerEntityArray = _mazeSpawnerQuery.ToEntityArray(Allocator.TempJob);
        var spawnerData = GetComponent<Spawner>(spawnerEntityArray[0]);
        
        //TODO: loop through all spawners to support multiple map spawning
        var tileData = GetBuffer<MapCell>(spawnerEntityArray[0]);
        
        var spawnJob = new MazeSpawnJob
        {
            MazeSize = spawnerData.MazeSize,
            Prefab = spawnerData.Prefab,
            ecb = parallelEcb,
            Tiles = tileData,
        };
        
        var spawnJobHandle =
            spawnJob.Schedule(spawnerData.MazeSize.x * spawnerData.MazeSize.y, 32, Dependency);


        Dependency = JobHandle.CombineDependencies(spawnJobHandle, Dependency);

        var cleanupJob = Entities.ForEach((Entity entity, in MazeSpawner mazeSpawner, in Spawner spawner) =>
        {
            ecb.RemoveComponent<MazeSpawner>(entity);
            ecb.RemoveComponent<Spawner>(entity);
            ecb.AddComponent<MazeTag>(entity);
        }).Schedule(Dependency);

        spawnerEntityArray.Dispose();
        
        Dependency = JobHandle.CombineDependencies(cleanupJob, Dependency);
        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}