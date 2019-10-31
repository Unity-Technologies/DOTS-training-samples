using GameAI;
using Rendering;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using static Unity.Mathematics.math;

namespace GameAI
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class HarvesterSpawnSystem : JobComponentSystem
    {
        // TODO: Remove hack for default archetype of new farmers once RenderingUnity.cs has been updated
        Entity defaultFarmerEntity;
        Entity defaultDroneEntity;
        int2 worldHalfSize;

        /// <summary>
        /// Spawns jobs for spawning new Entities that have the harvesting related components.
        /// </summary>
        /// <param name="inputDependencies">The previous aggregated input dependencies of previously run systems</param>
        /// <returns>The aggregated job handles from the spawn spawned in this job.</returns>
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            var ecb1 = ecbSystem.CreateCommandBuffer().ToConcurrent();

            var initSys = World.GetOrCreateSystem<RenderingMapInit>();
            
            defaultFarmerEntity = initSys.farmerEntityPrefab;
            defaultDroneEntity = initSys.droneEntityPrefab;
            
            worldHalfSize = World.GetOrCreateSystem<WorldCreatorSystem>().WorldSizeHalf;
            
            var defaultFarmer = defaultFarmerEntity;
            var defaultDrone = defaultDroneEntity;


            var worldHalfSizeLoc = worldHalfSize;
            
            // Spawn farmers
            var createFarmerJobHandle = Entities
                .WithAll<SpawnFarmerTagComponent>()
                // .WithoutBurst()
                .ForEach((int nativeThreadIndex, in SpawnPointComponent spawnPointData) =>
                {
                    var farmerEntity = ecb1.Instantiate(nativeThreadIndex, defaultFarmer);
                    var startWorldPosition = RenderingUnity.Tile2WorldPosition(spawnPointData.MapSpawnPosition, worldHalfSizeLoc);
                    ecb1.SetComponent<RenderingAnimationComponent>(
                        nativeThreadIndex, 
                        farmerEntity, 
                        new RenderingAnimationComponent { currentPosition = startWorldPosition.xz });
                    ecb1.SetComponent<Translation>(
                        nativeThreadIndex,
                        farmerEntity,
                        new Translation { Value = startWorldPosition });
                    ecb1.SetComponent<TilePositionable>(
                        nativeThreadIndex, 
                        farmerEntity, 
                        new TilePositionable { Position = spawnPointData.MapSpawnPosition });
                }).Schedule(inputDependencies);

            // Spawn drones
            var ecb2 = ecbSystem.CreateCommandBuffer().ToConcurrent();
            var createDroneJobHandle = Entities
                .WithAll<SpawnDroneTagComponent>()
                // .WithoutBurst()
                .ForEach((int nativeThreadIndex, Entity e, in SpawnPointComponent spawnPointData) =>
                    {
                        var droneEntity = ecb2.Instantiate(nativeThreadIndex, defaultDrone);
                        var startWorldPosition = RenderingUnity.Tile2WorldPosition(spawnPointData.MapSpawnPosition, worldHalfSizeLoc);
                        ecb2.SetComponent<RenderingAnimationComponent>(
                            nativeThreadIndex,
                            droneEntity,
                            new RenderingAnimationComponent { currentPosition = startWorldPosition.xz });
                        ecb2.SetComponent<Translation>(
                            nativeThreadIndex, 
                            droneEntity,
                            new Translation { Value = startWorldPosition });
                        ecb2.SetComponent<TilePositionable>(
                            nativeThreadIndex, 
                            droneEntity, 
                            new TilePositionable { Position = spawnPointData.MapSpawnPosition });
                    }).Schedule(inputDependencies);

            ecbSystem.AddJobHandleForProducer(createFarmerJobHandle);
            ecbSystem.AddJobHandleForProducer(createDroneJobHandle);

            // Aggregates the job handles with the previous jobs
            return JobHandle.CombineDependencies(createFarmerJobHandle, createDroneJobHandle);
        }
    }
}