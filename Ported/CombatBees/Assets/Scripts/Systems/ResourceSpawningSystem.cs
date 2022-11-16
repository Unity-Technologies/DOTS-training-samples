using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
partial struct ResourceSpawningSystem : ISystem
{
    public static readonly float RESOURCE_OBJ_HEIGHT = 0.5f;
    
    bool _hasInitialized;
    
    // Grid for stacking
    int2 _gridCounts;
    float2 _gridSize;
    float2 _minGridPos;
    
    // [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<ResourceConfig>();
        state.RequireForUpdate<ResourceConfig>();
        _hasInitialized = false;
        
        _gridCounts = new int2(new Vector2(Field.size.x,Field.size.z)/ RESOURCE_OBJ_HEIGHT);
        _gridSize = new float2(Field.size.x/_gridCounts.x,Field.size.z/_gridCounts.y);
        _minGridPos = new float2((_gridCounts.x-1f)*-.5f*_gridSize.x,(_gridCounts.y-1f)*-.5f*_gridSize.y);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    // [BurstCompile] disabled as it cannot access MouseCaster class. The method is only called once
    // a frame though, so no biggy. TJA
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<ResourceConfig>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        if (!_hasInitialized)
        {
            // This system will only run once, so the random seed can be hard-coded.
            // Using an arbitrary constant seed makes the behavior deterministic.
            var random = Random.CreateFromIndex(1234);

            var initialSpawnJob = new SpawnResourceJob()
            {
                ECB = ecb,
                ResourcePrefab = config.ResourcePrefab,
                SpawnCount = config.InitialCount,
                Random = random,
                IsPositionRandom = true,
                MinGridPos = _minGridPos,
                GridSize = _gridSize
            };

            var spawnHandle = initialSpawnJob.Schedule();
            spawnHandle.Complete();
            
            _hasInitialized = true;
        }

        // Spawn on click
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            var spawnOnClickJob = new SpawnResourceJob()
            {
                ECB = ecb,
                ResourcePrefab = config.ResourcePrefab,
                SpawnCount = config.InitialCount,
                Position = MouseRaycaster.worldMousePosition,
                IsPositionRandom = false,
            };

            var spawnHandle = spawnOnClickJob.Schedule();
            spawnHandle.Complete();
        }
    }
    
    [BurstCompile]
    struct SpawnResourceJob : IJob
    {
        public EntityCommandBuffer ECB;
        public Entity ResourcePrefab;
        public int SpawnCount;
        public Random Random;
        public bool IsPositionRandom;
        public float3 Position;
        public float2 MinGridPos;
        public float2 GridSize;
        public uint2 GridCounts;
        
        public void Execute()
        {
            var resources = CollectionHelper.CreateNativeArray<Entity>(SpawnCount, Allocator.Temp);
            ECB.Instantiate(ResourcePrefab, resources);

            foreach (var resource in resources)
            {
                var position = IsPositionRandom
                    // ALX: now using Field bounds, but could cluster closer to the centre if desired
                    // ? Random.NextFloat3(Field.ResourceBoundsMin, Field.ResourceBoundsMax)
                    ? new float3(MinGridPos.x * .25f + Random.NextFloat() * Field.size.x * .25f,Random.NextFloat() * 10f,MinGridPos.y + Random.NextFloat() * Field.size.z)
                    : Position;
                Initialize(resource, position);
            }
        }

        void Initialize(Entity resource, float3 position)
        {
            var uniformScaleTransform = new UniformScaleTransform
            {
                Position = position,
                Rotation = quaternion.identity,
                Scale = 1
            };
                
            // Set object position to position
            ECB.SetComponent(resource, new LocalToWorldTransform
            {
                Value = uniformScaleTransform
            });
            // Start off with an alive resource
            var resourceData = new Resource()
            {
                Dead = false,
                IsTopOfStack = true
            };
            ECB.AddComponent(resource, resourceData);
            // Set component data to position
            ECB.AddComponent(resource, new Physical()
            {
                Position = uniformScaleTransform.Position,
                Velocity = float3.zero,
                IsFalling = true,
                Collision = Physical.FieldCollisionType.Slump,
            });
            ECB.AddComponent(resource, new ResourceGatherable());
            ECB.SetComponentEnabled<ResourceGatherable>(resource, false);
            ECB.AddComponent(resource, new StackInProgress());
            ECB.SetComponentEnabled<StackInProgress>(resource, true);
        }
    }
}