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

    void Execute(Entity entity, TransformAspect transform, ref Target target)
    {
        float3 targetPosition = new float3(target.Value.x, 0.3f, target.Value.y);
        //Tiles[Get1DIndex(target.Value.x, target.Value.y);


        var distance = math.distance(targetPosition, transform.Position);
        if (distance > 0.5f)
        {
            transform.Position += math.normalize(targetPosition - transform.Position) * DeltaTime * 10; //Speed
        }
        else
        {
            var r = Unity.Mathematics.Random.CreateFromIndex(RandomSeed);
            target.Value = new int2(r.NextInt(0, Width), r.NextInt(0, Height));
        }
        //Random

        //Chasing player

        //Random location
    }
}


[BurstCompile]
public partial struct ZombieMovingSystem : ISystem
{
    private Unity.Mathematics.Random Random;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MazeConfig>();
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
            RandomSeed = Random.NextUInt()
        };
        moveJob.ScheduleParallel();
    }
}