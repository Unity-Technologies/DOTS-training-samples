using System.Runtime.CompilerServices;
using Ported.Scripts.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
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
       var DeltaTime = SystemAPI.Time.DeltaTime;
       
       
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
       

       
       /*
       foreach ((var transform, var unitMovement) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<UnitMovementComponent>>())
       {
           var targetPosition = unitMovement.ValueRO.targetPos;
        var direction = unitMovement.ValueRO.direction;
        var speed = unitMovement.ValueRO.speed;

        float3 targetPosition3D = new float3(targetPosition.x, transform.ValueRO.Position.y, targetPosition.y);

        RatLabHelper.DirectionToVector(out var dir, direction);

        //trying to use targetPos instead of direction
        //dir.x = (targetPosition.x - transform.LocalPosition.x);
        //dir.y = (targetPosition.y - transform.LocalPosition.z);

        bool done = false;
        
        if (dir.x > 0.0f && transform.ValueRO.Position.x > targetPosition.x)
        {
            done = true;
        }
        else if (dir.x < 0.0f && transform.ValueRO.Position.x < targetPosition.x)
        {
            done = true;
        }
        else if (dir.y > 0.0f && transform.ValueRO.Position.z > targetPosition.y)                    
        {                                                                                    
           done = true;                                                                     
        }                                                                                    
        else if (dir.y < 0.0f && transform.ValueRO.Position.z < targetPosition.y)               
        {                                                                                    
           done = true;                                                                     
        }                                                                                    
        
        Debug.Log($"done = {done}, dir = {dir}, targetPos = {targetPosition}, myPos = {transform.ValueRO.Position}");
        
        var dir3d = new float3(dir[0], 0.0f, dir[1]);

        if (!done)
        {
            transform.ValueRW.Position += (dir3d * speed * DeltaTime);    
        }
       }
       */
       
       
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
        var targetPosition = unitMovement.targetPos;
        var direction = unitMovement.direction;
        var speed = unitMovement.speed;

        float3 targetPosition3D = new float3(targetPosition.x, transform.LocalPosition.y, targetPosition.y);

        RatLabHelper.DirectionToVector(out var dir, direction);

        //trying to use targetPos instead of direction
        //dir.x = (targetPosition.x - transform.LocalPosition.x);
        //dir.y = (targetPosition.y - transform.LocalPosition.z);

        bool done = false;
        
        /*
        if (dir.x > 0.0f && transform.LocalPosition.x > targetPosition.x)
        {
            done = true;
        }
        else if (dir.x < 0.0f && transform.LocalPosition.x < targetPosition.x)
        {
            done = true;
        }
        else if (dir.y > 0.0f && transform.LocalPosition.z > targetPosition.y)                    
        {                                                                                    
           done = true;                                                                     
        }                                                                                    
        else if (dir.y < 0.0f && transform.LocalPosition.z < targetPosition.y)               
        {                                                                                    
           done = true;                                                                     
        } 
        */
        
        /*
        if ((transform.WorldPosition.x > targetPosition3D.x + (dir.x / 10000))
                && (transform.WorldPosition.z > targetPosition3D.z + (dir.y / 10000)))
        {
            done = true;
        }
        */
        
        //Debug.Log($"done = {done}, dir = {dir}, targetPos = {targetPosition}, myPos = {transform.WorldPosition}");
        
        var dir3d = new float3(dir[0], 0.0f, dir[1]);

        if (!done)
        {
            transform.LocalPosition += (dir3d * speed * DeltaTime);    
        }
    }
}