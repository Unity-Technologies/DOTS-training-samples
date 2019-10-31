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
        private Entity defaultFarmerEntity;
        private Entity defaultDroneEntity;

        /// <summary>
        /// When this system is created, generate default Harvester Entities in which to spawn via the
        /// base archetypes.
        /// </summary>
        protected override void OnCreate()
        {
            var initSys = World.GetOrCreateSystem<RenderingMapInit>();
            
            defaultFarmerEntity = initSys.farmerEntityPrefab;
            defaultDroneEntity = initSys.droneEntityPrefab;
        }

        /// <summary>
        /// Spawns jobs for spawning new Entities that have the harvesting related components.
        /// </summary>
        /// <param name="inputDependencies">The previous aggregated input dependencies of previously run systems</param>
        /// <returns>The aggregated job handles from the spawn spawned in this job.</returns>
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            var ecb1 = ecbSystem.CreateCommandBuffer().ToConcurrent();

            var defaultFarmer = defaultFarmerEntity;
            var defaultDrone = defaultDroneEntity;
            
            // Spawn farmers
            var createFarmerJobHandle = Entities
                .WithAll<SpawnFarmerTagComponent>()
    //            .WithoutBurst()
                .ForEach((int nativeThreadIndex, Entity e, in SpawnPointComponent spawnPointData) =>
                {
                    var farmerEntity = ecb1.Instantiate(nativeThreadIndex, defaultFarmer);
                    // TODO: Define what needs to be set on per entity basis, translate, scale? 
                    ecb1.SetComponent<Translation>(
                        nativeThreadIndex, 
                        farmerEntity, 
                        new Translation{ Value = spawnPointData.SpawnPoint});
                }).Schedule(inputDependencies);

            // Spawn drones
            var ecb2 = ecbSystem.CreateCommandBuffer().ToConcurrent();
            var createDroneJobHandle = Entities
                .WithAll<SpawnDroneTagComponent>()
    //            .WithoutBurst()
                .ForEach((int nativeThreadIndex, Entity e, in SpawnPointComponent spawnPointData) =>
                    {
                        // TODO:
                        var droneEntity = ecb2.Instantiate(nativeThreadIndex, defaultDrone);
                        ecb2.SetComponent<Translation>(
                            nativeThreadIndex, 
                            defaultDrone, 
                            new Translation{ Value = spawnPointData.SpawnPoint });
                    }).Schedule(inputDependencies);

            ecbSystem.AddJobHandleForProducer(createFarmerJobHandle);
            ecbSystem.AddJobHandleForProducer(createDroneJobHandle);

            // Aggregates the job handles with the previous jobs
            return JobHandle.CombineDependencies(createFarmerJobHandle, createDroneJobHandle);
        }
    }
}