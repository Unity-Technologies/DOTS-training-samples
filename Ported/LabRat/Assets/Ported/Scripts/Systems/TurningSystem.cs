using Ported.Scripts.Utils;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;

[BurstCompile]
public partial struct TurningSystem : ISystem
{
    private float counter;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        counter = 0.0f;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //check for movementUnits vs walls
        //if raycasting a wall, change the units direction
        
        /*
         *         foreach (var unitSpawner in SystemAPI.Query<RefRW<UnitSpawnerComponent>>())
        {
         */

        /*
        foreach ((var movement, var objPosition) in SystemAPI.Query<RefRW<UnitMovementComponent>, RefRO<WorldTransform>>())
        {
            //movement.ValueRW.direction = RatLabHelper.NextDirection(movement.ValueRO.direction);
              
            
            foreach ((var wall, var wallPosition) in SystemAPI.Query<RefRO<WallComponent>, RefRO<WorldTransform>>())
            {
                float2 unitPos = new float2(objPosition.ValueRO.Position.x, objPosition.ValueRO.Position.z);
                float2 wallPos = new float2(wallPosition.ValueRO.Position.x, wallPosition.ValueRO.Position.z);

                //Debug.Log($"Checking unitPos {objPosition.ValueRO.Position} versus wallPosition {wallPosition.ValueRO.Position}");
                
                //Debug.Log($"[MovementSystem] updated entities={numUpdatedEntities}");
                
                
                if (RatLabHelper.CollidesAABB(unitPos, wallPos, 5.0f, 5.0f))
                {
                    movement.ValueRW.direction = RatLabHelper.NextDirection(movement.ValueRO.direction);
                    break;
                }
            }
        }
        */
        counter += SystemAPI.Time.DeltaTime;

        if (counter > 4.0f)
        {
            /*
            foreach (var movement in SystemAPI.Query<RefRW<UnitMovementComponent>>())
            {
                movement.ValueRW.direction = RatLabHelper.NextDirection(movement.ValueRO.direction);
                
                foreach (var wall in SystemAPI.Query<RefRO<WallComponent>>())
                {
                
                }
            }

*/

            counter = 0.0f;
        }
    }
}

[BurstCompile]
// IJobEntity relies on source generation to implicitly define a query from the signature of the Execute function.
partial struct TurnUnitJob : IJobEntity
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

        //unitMovement.direction = ;
    }
}