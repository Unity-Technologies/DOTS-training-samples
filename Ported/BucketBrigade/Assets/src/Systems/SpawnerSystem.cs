using System;
using src.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace src.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class SpawnerSystem : SystemBase
    {
        const float mapSizeXZ = 100;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<FireSimConfig>();
            RequireSingletonForUpdate<FireSimConfigValues>();
        }

        protected override void OnUpdate()
        {
            var config = GetSingleton<FireSimConfig>();
            var configValues = GetSingleton<FireSimConfigValues>();

            // Fix configs to remove LinkedEntityGroup:
            EntityManager.RemoveComponent<LinkedEntityGroup>(config.OmniWorkerPrefab);
            EntityManager.RemoveComponent<LinkedEntityGroup>(config.BucketThrowerWorkerPrefab);
            EntityManager.RemoveComponent<LinkedEntityGroup>(config.FullBucketPasserWorkerPrefab);
            EntityManager.RemoveComponent<LinkedEntityGroup>(config.EmptyBucketPasserWorkerPrefab);
            EntityManager.RemoveComponent<LinkedEntityGroup>(config.BucketFetcherPrefab);
            EntityManager.RemoveComponent<LinkedEntityGroup>(config.BucketPrefab);
            
            // Spawn buckets:
            using var bucketEntities = new NativeArray<Entity>(configValues.BucketCount, Allocator.Temp);
            EntityManager.Instantiate(config.BucketPrefab, bucketEntities);
            for (var i = 0; i < bucketEntities.Length; i++)
            {
                var entity = bucketEntities[i];
                EntityManager.SetComponentData(entity, new Position
                {
                    Value = new float2(UnityEngine.Random.value * mapSizeXZ, UnityEngine.Random.value * mapSizeXZ),
                });  
                EntityManager.SetComponentData(entity, new EcsBucket
                {
                   WaterLevel = UnityEngine.Random.value,
                });
            }

            // Spawn teams:
            var teamContainerEntity = EntityManager.CreateEntity(ComponentType.ReadWrite<TeamData>());
            EntityManager.SetName(teamContainerEntity, "TeamContainer");
            var teamDataBuffer = EntityManager.GetBuffer<TeamData>(teamContainerEntity);
            for (int teamId = 0; teamId < configValues.TeamCount; teamId++)
            {
                teamDataBuffer.Add(new TeamData
                {
                    TargetFireCell = -1,
                });
            }
            
            // Spawn workers for those teams:
            for (int teamId = 0; teamId < configValues.TeamCount; teamId++)
            {
                SpawnPassers(config.FullBucketPasserWorkerPrefab, configValues.WorkerCountPerTeam, teamId);
                SpawnPassers(config.EmptyBucketPasserWorkerPrefab, configValues.WorkerCountPerTeam, teamId);
                SpawnThrower(config.BucketThrowerWorkerPrefab, teamId);
                SpawnFiller(config, teamId);
                SpawnFetcher(config, teamId);
            }

            // Once we've spawned, we can disable this system, as it's done its job.
            Enabled = false;
        }

        void SpawnFetcher(FireSimConfig config, int teamId)
        {
            const int numBucketFetcherWorkers = 1;
            using var bucketFetcherWorkers = new NativeArray<Entity>(numBucketFetcherWorkers, Allocator.Temp);
            EntityManager.Instantiate(config.BucketFetcherPrefab, bucketFetcherWorkers);
            for (var i = 0; i < bucketFetcherWorkers.Length; i++)
            {
                var entity = bucketFetcherWorkers[i];
                EntityManager.SetComponentData(entity, new Position
                {
                    Value = new float2(UnityEngine.Random.value * mapSizeXZ, UnityEngine.Random.value * mapSizeXZ),
                });
                EntityManager.SetComponentData(entity, new TeamId
                {
                    Id = teamId
                });
            }
        } 
        
        void SpawnFiller(FireSimConfig config, int teamId)
        {
            const int numBucketFetcherWorkers = 1;
            using var bucketFetcherWorkers = new NativeArray<Entity>(numBucketFetcherWorkers, Allocator.Temp);
            EntityManager.Instantiate(config.BucketFillerPrefab, bucketFetcherWorkers);
            for (var i = 0; i < bucketFetcherWorkers.Length; i++)
            {
                var entity = bucketFetcherWorkers[i];
                EntityManager.SetComponentData(entity, new Position
                {
                    Value = new float2(UnityEngine.Random.value * mapSizeXZ, UnityEngine.Random.value * mapSizeXZ),
                });
                EntityManager.SetComponentData(entity, new TeamId
                {
                    Id = teamId
                });
            }
        }

        private void SpawnPassers(Entity prefab, int count, int teamId)
        {
            using var passers = new NativeArray<Entity>(count, Allocator.Temp);
            EntityManager.Instantiate(prefab, passers);

            for (var f = 0; f < passers.Length; f++)
            {
                var entity = passers[f];
                SetTeamWorkerComponents(entity, teamId);
                EntityManager.SetComponentData(entity, new TeamPosition()
                {
                    Index = f
                });
            }
        }

        private void SpawnThrower(Entity prefab, int teamId)
        {
            using var throwers = new NativeArray<Entity>(1, Allocator.Temp);
            EntityManager.Instantiate(prefab, throwers);
            SetTeamWorkerComponents(throwers[0], teamId);
        }

        private void SetTeamWorkerComponents(Entity entity, int teamId)
        {
            EntityManager.SetComponentData(entity, new Position
            {
                Value = new float2(UnityEngine.Random.value * mapSizeXZ, UnityEngine.Random.value * mapSizeXZ),
            });

            EntityManager.SetComponentData(entity, new TeamId
            {
                Id = teamId,
            });
        }
    }
}
