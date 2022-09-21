using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = System.Random;

[BurstCompile]
[WithNone(typeof(ElevatingPosition))]
[WithAll(typeof(Zombie))]
partial struct ZombieMoveJob : IJobEntity
{
    public int Width;
    public int Height;
    public float DeltaTime;
    [ReadOnly] public DynamicBuffer<TileBufferElement> Tiles;
    public uint RandomSeed;
    public MazeConfig MazeConfig;

    void Execute(Entity entity, TransformAspect transform, ref Target target)
    {
        float3 targetPosition = new float3(target.TargetPosition.x, 0.3f, target.TargetPosition.y);
        
        var distance = math.distance(targetPosition, transform.Position);
        if (distance > 0.5f)
        {
            transform.Position += math.normalize(targetPosition - transform.Position) * DeltaTime * 10; //Speed
        }
        else
        {
            BreathFirstSearch(ref entity, ref transform, ref target);
            
        }
    }

    void BreathFirstSearch(ref Entity entity, ref TransformAspect transform, ref Target target)
    {
        //Random
        //Chasing player
        //Random location
        var r = Unity.Mathematics.Random.CreateFromIndex(RandomSeed);
        var newTarget = new int2(r.NextInt(0, Width), r.NextInt(0, Height));

        var path = new DynamicBuffer<int2>();
            
        //Breath first search

        MazeConfig.Get1DIndex(Width, Height);
            
        //NativeHashMap<int,float> result = new NativeHashMap<int,float>(1, Allocator.TempJob);
        target.TargetPosition = newTarget;
        target.TargetPath = path;
        target.PathIndex = 0;
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
    public uint RandomSeed;
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
            Unity.Mathematics.Random r = Unity.Mathematics.Random.CreateFromIndex(RandomSeed);
            TileBufferElement currentTIle = Tiles[MazeConfig.Get1DIndex(target.TargetPosition.x, target.TargetPosition.y)];

            int newDirection = r.NextInt(0, 4);
            switch(newDirection)
            {
                case 0:
                    if(!currentTIle.LeftWall && target.TargetPosition.x > 0)
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
            RandomSeed = Random.NextUInt(),
            MazeConfig =mazeConfig
        };
        moveJob.ScheduleParallel();

        var randomMoveJob = new RandomZombieMoveJob
        {
            Width = width,
            Height = height,
            Tiles = tiles,
            DeltaTime = deltaTime,
            RandomSeed = Random.NextUInt(),
            MazeConfig = mazeConfig
        };
        randomMoveJob.ScheduleParallel();
    }
}