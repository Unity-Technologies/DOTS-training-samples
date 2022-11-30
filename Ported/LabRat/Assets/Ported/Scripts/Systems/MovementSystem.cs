using System.Runtime.CompilerServices;
using Ported.Scripts.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;

[BurstCompile]
public partial struct MovementSystem : ISystem
{
   [BurstCompile]
   public void OnCreate(ref SystemState state)
   {
   }

   [BurstCompile]
   public void OnDestroy(ref SystemState state)
   {
   }

   [BurstCompile]
   public void OnUpdate(ref SystemState state)
   {
       /*
       var dt = SystemAPI.Time.DeltaTime;
       
       int numUpdatedEntities = 0;

       foreach (var (unit, pos, transform, entity) in SystemAPI.Query<RefRW<UnitMovementComponent>, RefRW<LocalToWorld>, RefRW<WorldTransform>>()
                    .WithEntityAccess())
       {
           numUpdatedEntities++;
           var direction = unit.ValueRO.direction;
           var speed = unit.ValueRO.speed;

           RatLabHelper.DirectionToVector(out var dir, direction);

           var dir3d = new float3(dir[0], 0.0f, dir[1]);
           
           var posTmp = pos.ValueRW.Position; 
           
            posTmp += (dir3d * speed * SystemAPI.Time.DeltaTime * 100.0f);
           // pos.ValueRW.position.x += 0.01f;

           // todo : check if possible to continue moving?
           // if(!CanMoveInDirection(...)) rotate(...)
           if (pos.ValueRO.Position.x > 10)
           {
               unit.ValueRW.direction = MovementDirection.West;
           }
           else if (pos.ValueRO.Position.x < -10)
           {
               unit.ValueRW.direction = MovementDirection.East;
           }

           transform.ValueRW.Position.x = posTmp.x;//pos.ValueRW.Position.x;
           transform.ValueRW.Position.y = posTmp.y; //pos.ValueRW.Position.y;
       }
       */
       
       
       var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
       var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
       var moveUnitJob = new MoveUnitJob()
       {
           // Note the function call required to get a parallel writer for an EntityCommandBuffer.
           ECB = ecb.AsParallelWriter(),
           // Time cannot be directly accessed from a job, so DeltaTime has to be passed in as a parameter.
           DeltaTime = SystemAPI.Time.DeltaTime
       };
       moveUnitJob.ScheduleParallel();

       //Debug.Log($"[MovementSystem] updated entities={numUpdatedEntities}");
   }
}

[BurstCompile]
// IJobEntity relies on source generation to implicitly define a query from the signature of the Execute function.
partial struct MoveUnitJob : IJobEntity
{
    // A regular EntityCommandBuffer cannot be used in parallel, a ParallelWriter has to be explicitly used.
    public EntityCommandBuffer.ParallelWriter ECB;
    // Time cannot be directly accessed from a job, so DeltaTime has to be passed in as a parameter.
    public float DeltaTime;

    // The ChunkIndexInQuery attributes maps the chunk index to an int parameter.
    // Each chunk can only be processed by a single thread, so those indices are unique to each thread.
    // They are also fully deterministic, regardless of the amounts of parallel processing happening.
    // So those indices are used as a sorting key when recording commands in the EntityCommandBuffer,
    // this way we ensure that the playback of commands is always deterministic.
    void Execute([ChunkIndexInQuery] int chunkIndex, ref TransformAspect transform, UnitMovementComponent unitMovement)
    {
        var direction = unitMovement.direction;
        var speed = unitMovement.speed;

        RatLabHelper.DirectionToVector(out var dir, direction);

        var dir3d = new float3(dir[0], 0.0f, dir[1]);
        
        transform.LocalPosition += (dir3d * speed * DeltaTime);
    }
}