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
    public static readonly float RESOURCE_OBJ_HEIGHT = 2f;

    bool _hasInitialized;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<ResourceConfig>();
        _hasInitialized = false;
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
            };

            var spawnHandle = initialSpawnJob.Schedule();
            spawnHandle.Complete();
            
            _hasInitialized = true;
        }

        // Spawn on click
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            var clickSpawnEcb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var spawnOnClickJob = new SpawnResourceJob()
            {
                ECB = clickSpawnEcb,
                ResourcePrefab = config.ResourcePrefab,
                SpawnCount = 1,
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
        
        public void Execute()
        {
            var resources = CollectionHelper.CreateNativeArray<Entity>(SpawnCount, Allocator.Temp);
            ECB.Instantiate(ResourcePrefab, resources);

            foreach (var resource in resources)
            {
                var position = IsPositionRandom
                    ? Random.NextFloat3(Field.ResourceBoundsMin, Field.ResourceBoundsMax)
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
                GridIndex = GetGridIndex(position)
            };

            ECB.AddComponent(resource, resourceData);
            // Set component data to position
            ECB.AddComponent(resource, new Physical()
            {
                Position = uniformScaleTransform.Position,
                Velocity = float3.zero,
                IsFalling = true,
                Collision = Physical.FieldCollisionType.Slump,
                SpeedModifier = 1f
            });
            ECB.AddComponent(resource, new ResourceGatherable());
            // Resources are gatherable in air
            ECB.SetComponentEnabled<ResourceGatherable>(resource, true);
            ECB.AddComponent(resource, new StackNeedsFix());
            ECB.SetComponentEnabled<StackNeedsFix>(resource, false);

            ECB.AddComponent(resource, new Dead());
            ECB.SetComponentEnabled<Dead>(resource, false);
        }
    
        int2 GetGridIndex(float3 pos) {
            int gridX = (int) math.round(pos.x);
            int gridZ = (int) math.round(pos.z);
            return new int2(gridX, gridZ);
        }
    }
}