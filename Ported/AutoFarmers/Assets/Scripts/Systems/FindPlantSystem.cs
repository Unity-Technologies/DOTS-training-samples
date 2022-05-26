using System.Resources;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Assets.Scripts.Systems.Drone
{
    [BurstCompile]
    public partial struct FindPlantSystem : ISystem
    {
        EntityQuery grownPlantsQuery;

        ComponentDataFromEntity<LocalToWorld> localToWorldFromEntity;

        ComponentDataFromEntity<Mover> moverFromEntity;

        ComponentDataFromEntity<Plant> plantFromEntity;

        public void OnCreate(ref SystemState state)
        {
            grownPlantsQuery = state.World.EntityManager.CreateEntityQuery(typeof(PlantGrown));
            localToWorldFromEntity = state.GetComponentDataFromEntity<LocalToWorld>(true);
            moverFromEntity = state.GetComponentDataFromEntity<Mover>();
            plantFromEntity = state.GetComponentDataFromEntity<Plant>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var chunks = grownPlantsQuery.CreateArchetypeChunkArray(Allocator.TempJob);
            if(chunks.Length == 0 || (chunks.Length == 1 && chunks[0].Count == 0))
            {
                return;
            }

            var claimedPlants = new NativeParallelHashMap<Entity, Entity>(100, Allocator.TempJob);
            var plantWritter = claimedPlants.AsParallelWriter();

            moverFromEntity.Update(ref state);
            localToWorldFromEntity.Update(ref state);
            plantFromEntity.Update(ref state);
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);//.AsParallelWriter();
            //foreach (var drone in SystemAPI.Query<DroneFindPlantAspect>())
            //{
            //    var dronePos = localToWorldFromEntity[drone.Self].Position;
            //    var closestPlant = Entity.Null;
            //    var closestPlantDistance = float.MaxValue;
            //    var closestPlantPos = new float3(0, 0, 0);
            //    for (int i = 0; i < chunks.Length; i++)
            //    {
            //        var chunk = chunks[i];
            //        var plants = chunk.GetNativeArray(state.GetEntityTypeHandle());
            //        for (int j = 0; j < chunk.Count; j++)
            //        {
            //            var plant = plants[j];
            //            var plantPos = localToWorldFromEntity[plant].Position;
            //            var claimed = plantFromEntity[plant].ClaimedBy;

            //            if (claimed != Entity.Null || claimedPlants.ContainsKey(plant)) 
            //            {
            //                continue;
            //            }
            //            var dist = math.distancesq(plantPos, dronePos);
            //            if (dist < closestPlantDistance)
            //            {
            //                closestPlantDistance = dist;
            //                closestPlant = plant;
            //                closestPlantPos = plantPos;
            //            }
            //        }
            //        plants.Dispose();
            //    }

            //    if (closestPlant != Entity.Null)
            //    {
            //        if(plantWritter.TryAdd(closestPlant, true))
            //        {
            //            ecb.SetComponent<Plant>(closestPlant, new Plant { ClaimedBy = drone.Self });
            //            ecb.AddComponent<DroneAquirePlantIntent>(drone.Self, new DroneAquirePlantIntent
            //            {
            //                Plant = closestPlant,
            //            });
            //            drone.DesiredLocation = new int2(
            //                (int)math.round(closestPlantPos.x),
            //                (int)math.round(closestPlantPos.z)
            //            );
            //            ecb.RemoveComponent<DroneFindPlantIntent>(drone.Self);
            //            ecb.RemoveComponent<PlantGrown>(closestPlant);
            //        }
            //    }
            //}

            var assignPlantsJob = new AssignPlantsJob
            {
                localToWorldFromEntity = localToWorldFromEntity,
                ecb = ecb,
                plantsToDroneMapping= claimedPlants,
                moverFromEntity= moverFromEntity,
            };

            var findPlantsJob = new FindPlantsJob
            {
                localToWorldFromEntity = localToWorldFromEntity,
                plantFromEntity = plantFromEntity,
                //claimedPlants = claimedPlants,
                plantWritter = plantWritter,
                //ecb = ecb,
                chunks = chunks,
                entityTypeHandle = state.GetEntityTypeHandle(),
            };
            
            state.Dependency = assignPlantsJob.Schedule(findPlantsJob.ScheduleParallel(state.Dependency));

            //chunks.Dispose();
            //claimedPlants.Dispose();
        }
    }


    [BurstCompile]
    partial struct AssignPlantsJob : IJob
    {
        [ReadOnly]
        public NativeParallelHashMap<Entity, Entity> plantsToDroneMapping;

        public EntityCommandBuffer ecb;

        [ReadOnly]
        public ComponentDataFromEntity<LocalToWorld> localToWorldFromEntity;

        [ReadOnly]
        public ComponentDataFromEntity<Mover> moverFromEntity;

        public void Execute()
        {
            foreach (var keyval in plantsToDroneMapping)
            {
                var plant = keyval.Key;
                var drone = keyval.Value;
                ecb.SetComponent<Plant>(plant, new Plant { ClaimedBy = drone });
                ecb.AddComponent<DroneAquirePlantIntent>(drone, new DroneAquirePlantIntent
                {
                    Plant = plant,
                });

                var plantPos = localToWorldFromEntity[plant].Position;
                var mover = moverFromEntity[drone];
                mover.DesiredLocation = new int2(
                    (int)math.round(plantPos.x),
                    (int)math.round(plantPos.z)
                );
                ecb.SetComponent<Mover>( drone, mover);
                ecb.RemoveComponent<DroneFindPlantIntent>( drone);
                ecb.RemoveComponent<PlantGrown>( plant);
            }
        }
    }

    [BurstCompile]
    partial struct FindPlantsJob : IJobEntity
    {
        [ReadOnly]
        public ComponentDataFromEntity<LocalToWorld> localToWorldFromEntity;

        [ReadOnly]
        public ComponentDataFromEntity<Plant> plantFromEntity;

        //public NativeParallelHashMap<Entity, bool> claimedPlants;

        public NativeParallelHashMap<Entity, Entity>.ParallelWriter plantWritter;

        //public EntityCommandBuffer.ParallelWriter ecb;

        [ReadOnly]
        public NativeArray<ArchetypeChunk> chunks;

        [ReadOnly]
        public EntityTypeHandle entityTypeHandle;

        void Execute(ref DroneFindPlantAspect drone)
        {
            var dronePos = localToWorldFromEntity[drone.Self].Position;
            var closestPlant = Entity.Null;
            var closestPlantDistance = float.MaxValue;
            //var closestPlantPos = new float3(0, 0, 0);
            for (int i = 0; i < chunks.Length; i++)
            {
                var chunk = chunks[i];
                var plants = chunk.GetNativeArray(entityTypeHandle);
                for (int j = 0; j < chunk.Count; j++)
                {
                    var plant = plants[j];
                    var plantPos = localToWorldFromEntity[plant].Position;
                    var claimed = plantFromEntity[plant].ClaimedBy;

                    if (claimed != Entity.Null) // || claimedPlants.ContainsKey(plant)
                    {
                        continue;
                    }
                    var dist = math.distancesq(plantPos, dronePos);
                    if (dist < closestPlantDistance)
                    {
                        closestPlantDistance = dist;
                        closestPlant = plant;
                        //closestPlantPos = plantPos;
                    }
                }
                plants.Dispose();
            }

            if (closestPlant != Entity.Null)
            {
                if (plantWritter.TryAdd(closestPlant, drone.Self))
                {
                    //ecb.SetComponent<Plant>(chunkIndex, closestPlant, new Plant { ClaimedBy = drone.Self });
                    //ecb.AddComponent<DroneAquirePlantIntent>(chunkIndex, drone.Self, new DroneAquirePlantIntent
                    //{
                    //    Plant = closestPlant,
                    //});
                    //drone.DesiredLocation = new int2(
                    //    (int)math.round(closestPlantPos.x),
                    //    (int)math.round(closestPlantPos.z)
                    //);
                    //ecb.RemoveComponent<DroneFindPlantIntent>(chunkIndex, drone.Self);
                    //ecb.RemoveComponent<PlantGrown>(chunkIndex, closestPlant);
                }
            }
        }
    }
}
