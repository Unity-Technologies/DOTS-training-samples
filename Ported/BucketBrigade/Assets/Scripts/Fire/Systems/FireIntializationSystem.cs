using System;
using System.Drawing;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class FireIntializationSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;
    
    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
        
        RequireSingletonForUpdate<FireSpawning>();
    }
    
    protected override void OnUpdate()
    {
        var Random = new Random(1);

        var ecb = m_CommandBufferSystem.CreateCommandBuffer();
        Entities.WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity configEntity, in FireConfiguration config, in FireSpawning spawning) =>
            { 
                var Size = config.GridHeight * config.GridHeight;
                
                var RandomTemperatures = new NativeArray<float>(Size, Allocator.TempJob);
                var GridEntries = new NativeArray<Entity>(Size, Allocator.TempJob);
                
                EntityManager.Instantiate(spawning.Prefab, GridEntries);

                // Randomize upfront (N cells that have non-zero temperature)
                for (int i = 0; i < Size; ++i)
                {
                    RandomTemperatures[i] = 0f;
                }
                for (int i = 0; i < config.NumInitialFires; ++i)
                {
                    int randomIndex = Random.NextInt(0, Size);
                    RandomTemperatures[randomIndex] = 0.2f;
                }

                // Set up entities
                var cellSize = config.CellSize;
                int index = 0;
                for (int z = 0; z < config.GridHeight; ++z)
                {
                    for (int x = 0; x < config.GridWidth; ++x)
                    {
                        var instance = GridEntries[index]; // ecb.Instantiate(spawning.Prefab);
                        var translation = new float3(x - (config.GridWidth - 1) / 2f, 0, z);
                        translation *= cellSize * 1.1f;
                        translation += config.Origin;

                        // Position
                        ecb.SetComponent(instance, new Translation { Value = translation });

                        // Temperatures
                        ecb.AddComponent(instance, new Temperature { Value = RandomTemperatures[index] } );
                        ecb.AddComponent(instance, new AddedTemperature { Value = 0 } );
 
                        // Set my Neighbors in a dynamic buffer; with e.g. maximum 8 neighbors if radius is 1
                        var neighbors = ecb.AddBuffer<FireGridNeighbor>(instance);

                        for (int xx = math.max(x - config.HeatRadius, 0); xx < config.GridWidth && xx <= x + config.HeatRadius; ++xx)
                        {
                            for (int zz = math.max(z - config.HeatRadius, 0); zz < config.GridHeight && zz <= z + config.HeatRadius; ++zz)
                            {
                                if (xx == x && zz == z)
                                    continue;

                                Entity someNeighbor = GridEntries[zz * config.GridHeight + xx];
                                neighbors.Add( new FireGridNeighbor { Entity = someNeighbor } );
                            }
                        }
                        
                        GridEntries[index] = instance;

                        index++;
                    }
                }

                ecb.RemoveComponent<FireSpawning>(configEntity);
                    
                RandomTemperatures.Dispose(Dependency);
                GridEntries.Dispose(Dependency);

            }).Run();
      
        m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
