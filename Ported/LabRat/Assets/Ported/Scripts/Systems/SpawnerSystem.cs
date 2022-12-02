using System;
using System.Linq;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using UnityEngine.UI;
using Unity.Mathematics;
using Unity.Burst.Intrinsics;
using Random = Unity.Mathematics.Random;

// Unmanaged systems based on ISystem can be Burst compiled, but this is not yet the default.
// So we have to explicitly opt into Burst compilation with the [BurstCompile] attribute.
// It has to be added on BOTH the struct AND the OnCreate/OnDestroy/OnUpdate functions to be
// effective.

[UpdateAfter(typeof(BoardSetupSystem)), BurstCompile]
partial struct SpawnerSystem : ISystem
{
    // A ComponentLookup provides random access to a component (looking up an entity).
    // We'll use it to extract the world space position and orientation of the spawn point (cannon nozzle).
    ComponentLookup<Unity.Transforms.LocalToWorld> m_LocalToWorldTransformFromEntity;
    private TileComponent startingTile;
    private bool setUp;
    
    // Every function defined by ISystem has to be implemented even if empty.
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        setUp = false;
        // ComponentLookup structures have to be initialized once.
        // The parameter specifies if the lookups will be read only or if they should allow writes.
        m_LocalToWorldTransformFromEntity = state.GetComponentLookup<LocalToWorld>(true);
    }

    // Every function defined by ISystem has to be implemented even if empty.
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    // See note above regarding the [BurstCompile] attribute.
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //Instead of caching (as an option)
        //SystemAPI.GetComponentLookup<LocalTransform>();
        m_LocalToWorldTransformFromEntity.Update(ref state);

        if (!setUp)
        {
            bool foundComponents = false;
            
            //Getting the nearest tile
            int unitSpawnersSetUp = 0;
            
            foreach ((var unitSpawner, var spawnerTransform) in SystemAPI.Query<RefRW<UnitSpawnerComponent>, RefRO<LocalTransform>>())
            {
                foreach ((var tile, var transform) in SystemAPI.Query<RefRW<TileComponent>, RefRO<LocalTransform>>())
                {
                    foundComponents = true;
                    
                    float distanceX = (transform.ValueRO.Position.x - spawnerTransform.ValueRO.Position.x);
                    float distanceZ = (transform.ValueRO.Position.z - spawnerTransform.ValueRO.Position.z);
                    if (distanceX < 0.0f) { distanceX *= -1.0f; }
                    if (distanceZ < 0.0f) { distanceZ *= -1.0f; }
                    
//                    Debug.LogError($"DistanceX = {distanceX}, DistanceZ = {distanceZ}");
                
                    if (distanceX < 1.0f && distanceZ < 1.0f)
                    {
                        //Debug.LogError($"Set up unity spawner {unitSpawnersSetUp}");
                        unitSpawner.ValueRW.startTileReference = tile.ValueRO;
                        //unitSpawnersSetUp++;
                        break;
                    }
                }
            }
            if (foundComponents)
            {
                setUp = true;    
            }
        }

        if (!setUp)
        {
            return;
        }
        
        //Debug.Log("SpawnerSystem OnUpdate");
        
        float dt = SystemAPI.Time.DeltaTime;

        NativeList<SpawnUnit> array = new NativeList<SpawnUnit>(state.WorldUpdateAllocator);

        uint numIterations = 1;
        
        foreach (var unitSpawner in SystemAPI.Query<RefRW<UnitSpawnerComponent>>())
        {
            numIterations++;
            unitSpawner.ValueRW.counter += dt;
            
            if (unitSpawner.ValueRO.counter > unitSpawner.ValueRO.frequency)
            {
                //Registering jobs for the entities to spawn
                var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
                
                for (int i = 0; i < unitSpawner.ValueRO.max; i++)
                {
                    numIterations++;
                
                    var random = Unity.Mathematics.Random.CreateFromIndex(numIterations);
                    var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
                    float3 wantedPos = new float3();
                    int2 wantedIndex = unitSpawner.ValueRO.startTileReference.nextTile;

                    switch (unitSpawner.ValueRO.startDirection)
                    {
                        case MovementDirection.East:
                            wantedIndex = unitSpawner.ValueRO.startTileReference.eastTile;
                            break;
                        case MovementDirection.West:
                            wantedIndex = unitSpawner.ValueRO.startTileReference.westTile;
                            break;
                        case MovementDirection.North:
                            wantedIndex = unitSpawner.ValueRO.startTileReference.northTile;
                            break;
                        case MovementDirection.South:
                            wantedIndex = unitSpawner.ValueRO.startTileReference.southTile;
                            break;
                    }

                    int startDir = (int)unitSpawner.ValueRO.startDirection;
                    
                    //Debug.LogError($"starting on index = {unitSpawner.ValueRO.startTileReference.gridIndex}, wanted index = {wantedIndex}, unitSpawner.startDirection = {startDir}");   
                    
                    foreach ((var tile, var transform) in SystemAPI.Query<RefRO<TileComponent>, RefRO<LocalTransform>>())
                    {
                        if (tile.ValueRO.gridIndex.x == wantedIndex.x
                            && tile.ValueRO.gridIndex.y == wantedIndex.y)
                        {
                            wantedPos = transform.ValueRO.Position;
                            break;
                        }
                    }

                    var spawnJob = new SpawnUnit
                    {
                        LocalToWorldTransformFromEntity = m_LocalToWorldTransformFromEntity,
                        ECB = ecb,
                        randomSeed = random.NextUInt(0, UInt32.MaxValue),
                        wantedIndex = wantedIndex,
                        wantedPosition = wantedPos
                    };
                    array.Add(spawnJob);
                }

                unitSpawner.ValueRW.counter = 0.0f;
            }
        }

        for (int i = 0; i < array.Length; i++)
        {
            var job = array[i];
            job.Schedule();
        }
        
        
    }
}

// Requiring the Shooting tag component effectively prevents this job from running
// for the tanks which are in the safe zone.
//[WithAll(typeof(Shooting))]
[BurstCompile]
partial struct SpawnUnit : IJobEntity
{
    [ReadOnly] public ComponentLookup<Unity.Transforms.LocalToWorld> LocalToWorldTransformFromEntity;
    public EntityCommandBuffer ECB;
    public uint randomSeed;

    public int2 wantedIndex;
    public float3 wantedPosition;
    
    /*
    bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        Debug.Log("on chunk begin");

        return true;
    }
    */
    
    void Execute([ChunkIndexInQuery] int chunkIndex, in UnitSpawnerComponent unit)
    {
        var instance = ECB.Instantiate(unit.spawnObject);
        var spawnLocalToWorld = LocalToWorldTransformFromEntity[unit.spawnPoint].Position;
        var spawnTransform = LocalTransform.FromPosition(spawnLocalToWorld);

        //var random = Unity.Mathematics.Random.CreateFromIndex((uint)instance.Index);
        //Debug.Log($"Getting random seed with with idx={randomSeed}");
        
        var random = Unity.Mathematics.Random.CreateFromIndex((uint)randomSeed);

        ECB.SetComponent(instance, spawnTransform);
        ECB.SetComponent(instance, new UnitMovementComponent
        {
            speed = random.NextFloat(unit.minSpeed, unit.maxSpeed), 
            direction = unit.startDirection, 
            targetTile = wantedIndex, 
            targetPos = new float2(wantedPosition.x, wantedPosition.z)
        } );
    }
}