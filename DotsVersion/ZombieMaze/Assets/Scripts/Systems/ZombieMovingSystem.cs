using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Profiling;
using Random = System.Random;

[BurstCompile]
[WithNone(typeof(ElevatingPosition))]
[WithAll(typeof(Zombie))]
partial struct ZombieMoveJob : IJobEntity
{
    private static readonly ProfilerMarker StaticMarker = new ProfilerMarker("TestStaticBurst");
    public int Width;
    public int Height;
    public float DeltaTime;
    [ReadOnly] public DynamicBuffer<TileBufferElement> Tiles;
    public Unity.Mathematics.Random Rand;
    public uint RandomSeed;
    public MazeConfig MazeConfig;
    public float3 PlayerPosition;

    [BurstCompile]
    void Execute(Entity entity, TransformAspect transform, ref Target target)
    {
        if (target.PathIndex < 0 || target.TargetPath.IsEmpty)
        {
            var p = new ProfilerMarker("TestBurst");
            using (StaticMarker.Auto())
            {
                p.Begin();
                //BreadthFirstSearch(ref entity, ref transform, ref target);
                BreadthFirstSearchArray(ref entity, ref transform, ref target);
                p.End();
            }
        }
        else
        {
            var nextPosition = target.TargetPath[target.PathIndex];

            float3 targetPosition = new float3(nextPosition.x, 0.3f, nextPosition.y);
            var distance = math.distance(targetPosition, transform.Position);

            var currentTile = target.PathIndex == (target.TargetPath.Length - 1)
                ? nextPosition
                : target.TargetPath[target.PathIndex + 1];

            var direction = nextPosition - currentTile;
            var tile = Tiles[Get1DIndex(currentTile)];

            if (direction.x > 0 && tile.RightWall)
            {
                return;
            }

            if (direction.x < 0 && tile.LeftWall)
            {
                return;
            }

            if (direction.y > 0 && tile.UpWall)
            {
                return;
            }

            if (direction.y < 0 && tile.DownWall)
            {
                return;
            }

            if (distance > 0.1f)
            {
                float speed = DeltaTime * 20;
                if (speed > distance)
                {
                    transform.Position = targetPosition;
                    target.PathIndex--;
                }
                else
                {
                    transform.Position += math.normalize(targetPosition - transform.Position) * speed;
                }
            }
            else
            {
                target.PathIndex--;
            }
        }
    }

    [BurstCompile]
    void BreadthFirstSearch(ref Entity entity, ref TransformAspect transform, ref Target target)
    {
        //Random location
        var p1 = new ProfilerMarker("TestBurst Create");
        p1.Begin();

        var newTarget = new int2(Rand.NextInt(0, Width), Rand.NextInt(0, Height));

        NativeHashMap<int2, int2>
            visited = new NativeHashMap<int2, int2>((Width + 1) * (Height + 1), Allocator.TempJob);


        NativeQueue<int2> toVisit = new NativeQueue<int2>(Allocator.TempJob);
        var currentPostion = target.TargetPosition;
        toVisit.Enqueue(currentPostion);

        visited.Add(currentPostion, currentPostion);

        var pathFound = false;
        p1.End();
        var p2 = new ProfilerMarker("TestBurst Loop");
        p2.Begin();
        while (!toVisit.IsEmpty())
        {
            var tileCoord = toVisit.Dequeue();
            if (tileCoord.Equals(newTarget))
            {
                pathFound = true;
                break;
            }

            var tile = Tiles[MazeConfig.Get1DIndex(tileCoord.x, tileCoord.y)];

            var newTile = tileCoord - new int2(1, 0);
            if (newTile.x >= 0 && !tile.LeftWall && !visited.ContainsKey(newTile))
            {
                toVisit.Enqueue(newTile);
                visited.Add(newTile, tileCoord);
            }

            newTile = tileCoord + new int2(1, 0);
            if (newTile.x <= Width && !tile.RightWall && !visited.ContainsKey(newTile))
            {
                toVisit.Enqueue(newTile);
                visited.Add(newTile, tileCoord);
            }

            newTile = tileCoord + new int2(0, 1);
            if (newTile.y <= Height && !tile.UpWall && !visited.ContainsKey(newTile))
            {
                toVisit.Enqueue(newTile);
                visited.Add(newTile, tileCoord);
            }

            newTile = tileCoord - new int2(0, 1);
            if (newTile.y >= 0 && !tile.DownWall && !visited.ContainsKey(newTile))
            {
                toVisit.Enqueue(newTile);
                visited.Add(newTile, tileCoord);
            }
        }

        p2.End();

        var p3 = new ProfilerMarker("TestBurst GenPath");
        p3.Begin();
        //Generate the path
        var path = new NativeList<int2>(Allocator.Persistent);
        if (pathFound)
        {
            path.Add(newTarget);
            var nextPosition = visited[newTarget];
            while (!currentPostion.Equals(nextPosition))
            {
                path.Add(nextPosition);
                nextPosition = visited[nextPosition];
            }

            path.Add(currentPostion);
        }

        p3.End();
        target.TargetPosition = newTarget;
        target.TargetPath = path;
        target.PathIndex = path.Length - 1;
    }

    [BurstCompile]
    void BreadthFirstSearchArray(ref Entity entity, ref TransformAspect transform, ref Target target)
    {
        //Random location
        var result = Rand.NextUInt(RandomSeed) % 2;
        if (!target.TargetPlayer)
        {
            result = 0;
        }

        int2 newTarget = new int2();
        switch (result)
        {
            case 0:
                newTarget = new int2(Rand.NextInt(0, Width), Rand.NextInt(0, Height));
                break;
            case 1:
            default:
                newTarget = new int2((int)Math.Round(PlayerPosition.x), (int)Math.Round(PlayerPosition.z));
                break;
        }

        NativeArray<int2> parentTile = new NativeArray<int2>((Width + 1) * (Height + 1), Allocator.Temp);
        NativeArray<bool> visited = new NativeArray<bool>((Width + 1) * (Height + 1), Allocator.Temp);
        NativeQueue<int2> toVisit = new NativeQueue<int2>(Allocator.TempJob);
        
        var currentPostion = new int2((int)Math.Round(transform.Position.x), (int)Math.Round(transform.Position.z));
        toVisit.Enqueue(currentPostion);

        parentTile[Get1DIndex(currentPostion)] = currentPostion;
        visited[Get1DIndex(currentPostion)] = true;

        var pathFound = false;
        while (!toVisit.IsEmpty())
        {
            var tileCoord = toVisit.Dequeue();
            if (tileCoord.Equals(newTarget))
            {
                pathFound = true;
                break;
            }

            var tile = Tiles[MazeConfig.Get1DIndex(tileCoord.x, tileCoord.y)];

            var newTile = tileCoord - new int2(1, 0);
            if (newTile.x >= 0 && !tile.LeftWall && !visited[Get1DIndex(newTile)])
            {
                toVisit.Enqueue(newTile);
                visited[Get1DIndex(newTile)] = true;
                parentTile[Get1DIndex(newTile)] = tileCoord;
            }

            newTile = tileCoord + new int2(1, 0);
            if (newTile.x <= Width && !tile.RightWall && !visited[Get1DIndex(newTile)])
            {
                toVisit.Enqueue(newTile);
                visited[Get1DIndex(newTile)] = true;
                parentTile[Get1DIndex(newTile)] = tileCoord;
            }

            newTile = tileCoord + new int2(0, 1);
            if (newTile.y <= Height && !tile.UpWall && !visited[Get1DIndex(newTile)])
            {
                toVisit.Enqueue(newTile);
                visited[Get1DIndex(newTile)] = true;
                parentTile[Get1DIndex(newTile)] = tileCoord;
            }

            newTile = tileCoord - new int2(0, 1);
            if (newTile.y >= 0 && !tile.DownWall && !visited[Get1DIndex(newTile)])
            {
                toVisit.Enqueue(newTile);
                visited[Get1DIndex(newTile)] = true;
                parentTile[Get1DIndex(newTile)] = tileCoord;
            }
        }

        //Generate the path
        var path = new NativeList<int2>(Allocator.Persistent);
        if (pathFound)
        {
            path.Add(newTarget);
            var nextPosition = parentTile[Get1DIndex(newTarget)];
            while (!currentPostion.Equals(nextPosition))
            {
                path.Add(nextPosition);
                nextPosition = parentTile[Get1DIndex(nextPosition)];
            }

            path.Add(currentPostion);
        }

        target.TargetPosition = newTarget;
        target.TargetPath = path;
        target.PathIndex = path.Length - 1;
    }

    public int Get1DIndex(int2 index)
    {
        return index.x + index.y * (Width + 1);
    }
}

[BurstCompile]
[WithNone(typeof(ElevatingPosition))]
[WithAll(typeof(Zombie))]
partial struct RandomZombieMoveJob : IJobEntity
{
    public int Width;
    public int Height;
    public float DeltaTime;
    [ReadOnly] public DynamicBuffer<TileBufferElement> Tiles;
    public Unity.Mathematics.Random Rand;
    public MazeConfig MazeConfig;

    void Execute(Entity entity, TransformAspect transform, ref RandomMovement target)
    {
        float3 targetPosition = new float3(target.TargetPosition.x, 0.3f, target.TargetPosition.y);

        var distance = math.distance(targetPosition, transform.Position);
        if (distance > 0.1f)
        {
            transform.Position += math.normalize(targetPosition - transform.Position) * DeltaTime * 1; //Speed
        }
        else
        {
            TileBufferElement currentTIle =
                Tiles[MazeConfig.Get1DIndex(target.TargetPosition.x, target.TargetPosition.y)];

            int newDirection = Rand.NextInt(0, 4);
            switch (newDirection)
            {
                case 0:
                    if (!currentTIle.LeftWall && target.TargetPosition.x > 0)
                    {
                        target.TargetPosition += new int2(-1, 0);
                    }

                    break;
                case 1:
                    if (!currentTIle.RightWall && target.TargetPosition.x < Width)
                    {
                        target.TargetPosition += new int2(1, 0);
                    }

                    break;
                case 2:
                    if (!currentTIle.UpWall && target.TargetPosition.y < Height)
                    {
                        target.TargetPosition += new int2(0, 1);
                    }

                    break;
                case 3:
                    if (!currentTIle.DownWall && target.TargetPosition.y > 0)
                    {
                        target.TargetPosition += new int2(0, -1);
                    }

                    break;
            }
        }
    }
}


[CreateAfter(typeof(TileCreationSystem))]
[UpdateAfter(typeof(MovingWallMovement))]
[BurstCompile]
public partial struct ZombieMovingSystem : ISystem
{
    private Unity.Mathematics.Random Random;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MazeConfig>();
        state.RequireForUpdate<TileBufferElement>();
        state.RequireForUpdate<PlayerData>();
        Random = Unity.Mathematics.Random.CreateFromIndex(1234);
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        MazeConfig mazeConfig = SystemAPI.GetSingleton<MazeConfig>();

        int width = mazeConfig.Width - 1;
        int height = mazeConfig.Height - 1;
        var tiles = SystemAPI.GetSingletonBuffer<TileBufferElement>();
        var player = SystemAPI.GetSingletonEntity<PlayerData>();
        
        //Draw Maze data to debug
        bool mazeDataDebug = true;
        if (mazeDataDebug)
        {
            float drawHeight = 0.7f;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var tile = tiles[mazeConfig.Get1DIndex(i, j)];
                    if (tile.LeftWall)
                        UnityEngine.Debug.DrawLine(
                            new Vector3(i - 0.5f, drawHeight, j - 0.5f),
                            new Vector3(i - 0.5f, drawHeight, j + 0.5f));
                    if (tile.RightWall)
                        UnityEngine.Debug.DrawLine(
                            new Vector3(i + 0.5f, drawHeight, j - 0.5f),
                            new Vector3(i + 0.5f, drawHeight, j + 0.5f));
                    if (tile.UpWall)
                        UnityEngine.Debug.DrawLine(
                            new Vector3(i - 0.5f, drawHeight, j + 0.5f),
                            new Vector3(i + 0.5f, drawHeight, j + 0.5f));
                    if (tile.DownWall)
                        UnityEngine.Debug.DrawLine(
                            new Vector3(i - 0.5f, drawHeight, j - 0.5f),
                            new Vector3(i + 0.5f, drawHeight, j - 0.5f));
                }
            }
        }

        var deltaTime = Time.deltaTime;

        var playerPos = SystemAPI.GetAspectRO<TransformAspect>(player);
        var moveJob = new ZombieMoveJob
        {
            Width = width,
            Height = height,
            Tiles = tiles,
            DeltaTime = deltaTime,
            Rand = Random,
            RandomSeed = Random.NextUInt(0, 9),
            MazeConfig = mazeConfig,
            PlayerPosition = playerPos.Position
        };

        moveJob.ScheduleParallel();

        var randomMoveJob = new RandomZombieMoveJob
        {
            Width = width,
            Height = height,
            Tiles = tiles,
            DeltaTime = deltaTime,
            Rand = Random,
            MazeConfig = mazeConfig
        };

        randomMoveJob.ScheduleParallel();
    }
}