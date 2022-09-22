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
    public MazeConfig MazeConfig;

    //[BurstCompile]
    void Execute(Entity entity, TransformAspect transform, ref Target target)
    {
        //Todo check for wall change
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
            if (distance > 0.5f)
            {
                transform.Position += math.normalize(targetPosition - transform.Position) * DeltaTime * 2; //Speed
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
        
        var newTarget = new int2(Rand.NextInt(0, Width), Rand.NextInt(0, Height));

        NativeArray<int2> visited2 = new NativeArray<int2>((Width + 1) * (Height + 1), Allocator.Temp);
        NativeArray<bool> visited3 = new NativeArray<bool>((Width + 1) * (Height + 1), Allocator.Temp);

        NativeQueue<int2> toVisit = new NativeQueue<int2>(Allocator.TempJob);
        var currentPostion = target.TargetPosition;
        toVisit.Enqueue(currentPostion);

        visited2[Get1DIndex(currentPostion)] = currentPostion;
        visited3[Get1DIndex(currentPostion)] = true;

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
            if (newTile.x >= 0 && !tile.LeftWall && !visited3[Get1DIndex(newTile)])
            {
                toVisit.Enqueue(newTile);
                visited3[Get1DIndex(newTile)] = true;
                visited2[Get1DIndex(newTile)] = tileCoord;
            }

            newTile = tileCoord + new int2(1, 0);
            if (newTile.x <= Width && !tile.RightWall && !visited3[Get1DIndex(newTile)])
            {
                toVisit.Enqueue(newTile);
                visited3[Get1DIndex(newTile)] = true;
                visited2[Get1DIndex(newTile)] = tileCoord;
            }

            newTile = tileCoord + new int2(0, 1);
            if (newTile.y <= Height && !tile.UpWall && !visited3[Get1DIndex(newTile)])
            {
                toVisit.Enqueue(newTile);
                visited3[Get1DIndex(newTile)] = true;
                visited2[Get1DIndex(newTile)] = tileCoord;
            }

            newTile = tileCoord - new int2(0, 1);
            if (newTile.y >= 0 && !tile.DownWall && !visited3[Get1DIndex(newTile)])
            {
                toVisit.Enqueue(newTile);
                visited3[Get1DIndex(newTile)] = true;
                visited2[Get1DIndex(newTile)] = tileCoord;
            }
        }

        //Generate the path
        var path = new NativeList<int2>(Allocator.Persistent);
        if (pathFound)
        {
            path.Add(newTarget);
            var nextPosition = visited2[Get1DIndex(newTarget)];
            while (!currentPostion.Equals(nextPosition))
            {
                path.Add(nextPosition);
                nextPosition = visited2[Get1DIndex(nextPosition)];
            }

            path.Add(currentPostion);
        }

        target.TargetPosition = newTarget;
        target.TargetPath = path;
        target.PathIndex = path.Length - 1;
    }

    public int Get1DIndex(int2 index)
    {
        return index.x + index.y * Width;
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


[BurstCompile]
public partial struct ZombieMovingSystem : ISystem
{
    private Unity.Mathematics.Random Random;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MazeConfig>();
        state.RequireForUpdate<TileBufferElement>();
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

        var deltaTime = Time.deltaTime;
        var moveJob = new ZombieMoveJob
        {
            Width = width,
            Height = height,
            Tiles = tiles,
            DeltaTime = deltaTime,
            Rand= Random,
            MazeConfig = mazeConfig
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