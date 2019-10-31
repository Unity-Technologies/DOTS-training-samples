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
            // var farmerSpawner1 = EntityManager.CreateEntity(typeof(SpawnPointComponent), typeof(SpawnFarmerTagComponent));
            // EntityManager.SetComponentData(farmerSpawner1, new SpawnPointComponent {MapSpawnPosition = new int2(3, 4)});

            var defaultFarmerArchetype = EntityManager.CreateArchetype(
                // TODO: Add additional components movement components for map
                typeof(NonUniformScale),
                typeof(Translation),
                typeof(LocalToWorld),
                typeof(RenderMesh),
                typeof(RenderingAnimationComponent),
                typeof(FarmerAITag),
                typeof(AITagTaskNone));

            var farmerMeshRenderer = RenderingUnity.instance.farmer;
            defaultFarmerEntity = EntityManager.CreateEntity(defaultFarmerArchetype);
            var farmerRenderMesh = new RenderMesh
            {
                mesh = farmerMeshRenderer.GetComponent<MeshFilter>().sharedMesh,
                material = farmerMeshRenderer.sharedMaterial,
                castShadows = farmerMeshRenderer.shadowCastingMode,
                // TODO: Set back once performance is proven.
                receiveShadows = false
            };
            EntityManager.SetSharedComponentData<RenderMesh>(defaultFarmerEntity, farmerRenderMesh);
            EntityManager.SetComponentData(defaultFarmerEntity,
                new NonUniformScale {Value = farmerMeshRenderer.transform.localScale});

            var defaultDroneArchetype = EntityManager.CreateArchetype(
                // TODO: Add additional components movement components for map
                typeof(NonUniformScale), 
                typeof(Translation),
                typeof(LocalToWorld), 
                typeof(RenderMesh), 
                typeof(RenderingAnimationComponent),
                typeof(RenderingAnimationDroneFlyComponent),
                typeof(AITagTaskNone));
            
            var droneMeshRenderer = RenderingUnity.instance.drone;
            defaultDroneEntity = EntityManager.CreateEntity(defaultDroneArchetype);
            var droneRenderMesh = new RenderMesh
            {
                mesh = droneMeshRenderer.GetComponent<MeshFilter>().sharedMesh,
                material = droneMeshRenderer.sharedMaterial,
                castShadows = droneMeshRenderer.shadowCastingMode,
                // TODO: Set back once performance is proven.
                receiveShadows = false
            };
            EntityManager.SetSharedComponentData<RenderMesh>(defaultDroneEntity, droneRenderMesh);
            EntityManager.SetComponentData(defaultFarmerEntity,
                new NonUniformScale {Value = droneMeshRenderer.transform.localScale});

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
            var createFarmerJobHandles = Entities
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
            var createDroneJobHandles = Entities
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
                    }).Schedule(inputDependencies /*TODO: should use default?*/);

            ecbSystem.AddJobHandleForProducer(createFarmerJobHandles);
            ecbSystem.AddJobHandleForProducer(createDroneJobHandles)
            // Aggregates the job handles with the previous jobs
            return JobHandle.CombineDependencies(inputDependencies, createFarmerJobHandles, createDroneJobHandles);
        }
    }
}