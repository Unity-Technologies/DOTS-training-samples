using System.Linq.Expressions;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public struct DestroySpawningRequestJob : IJobChunk
{
    [ReadOnly] public EntityTypeHandle EntityHandle;
    
    public EntityCommandBuffer.ParallelWriter ECB;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        var chunkEntityEnumerator = new ChunkEntityEnumerator(useEnabledMask, chunkEnabledMask, chunk.ChunkEntityCount);
        var entities = chunk.GetNativeArray(EntityHandle);

        while (chunkEntityEnumerator.NextEntityIndex(out var i))
        {
            var entity = entities[i];
            ECB.DestroyEntity(i, entity);
        }
    }
}

partial struct SpawningRequestSystem : ISystem
{
    private EntityQuery SpawningRequestQuery;
    private EntityQuery MeshRendererQuery;
    public EntityTypeHandle EntityHandle;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        SpawningRequestQuery = SystemAPI.QueryBuilder().WithAll<SpawningRequest>().Build();
        MeshRendererQuery = SystemAPI.QueryBuilder().WithAll<RenderMeshArray>().Build();
        
        // Only need to update if there are any entities with a SpawnRequestQuery
        state.RequireForUpdate(SpawningRequestQuery);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<BeeConfig>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

        var combinedSpawnJobHandle = new JobHandle();
        var combinedDestroyJobHandle = new JobHandle();

        var spawningRequestEntities = SpawningRequestQuery.ToEntityArray(Allocator.Temp);
        foreach (var spawningRequestEntity in spawningRequestEntities)
        {
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var spawningRequest = state.EntityManager.GetComponentData<SpawningRequest>(spawningRequestEntity);
            var transform = state.EntityManager.GetComponentData<LocalToWorldTransform>(spawningRequest.Prefab);
            
            var beeSpawnJob = new BeeSpawnJob
            {
                // Note the function call required to get a parallel writer for an EntityCommandBuffer.
                ECB = ecb.AsParallelWriter(),

                Aabb = spawningRequest.Aabb,
                Prefab = spawningRequest.Prefab,
                InitTransform = transform,
                InitFaction = spawningRequest.Faction,
                InitColor = spawningRequest.Color,
                Mask = MeshRendererQuery.GetEntityQueryMask(),
                InitVel = spawningRequest.InitVelocity,
                Aggressivity= config.aggressivity,
            };
            var spawnJobHandle = beeSpawnJob.Schedule(spawningRequest.Count, 64, state.Dependency);
            combinedSpawnJobHandle = JobHandle.CombineDependencies(spawnJobHandle, combinedSpawnJobHandle);
        }
        
        EntityHandle.Update(ref state);
        var ecb2 = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var destroySpawningRequestJob = new DestroySpawningRequestJob
        {
            ECB = ecb2.AsParallelWriter(),
            EntityHandle = EntityHandle
        };
        var destroySpawningRequestJobHandle = destroySpawningRequestJob.ScheduleParallel(SpawningRequestQuery, combinedSpawnJobHandle);
        
        // establish dependency between the spawn job and the command buffer to ensure the spawn job is completed
        // before the command buffer is played back.
        state.Dependency = destroySpawningRequestJobHandle;
    }

    public struct SpawningRequestInfo
    {
        public Entity Prefab;
        public int Faction;
        public float3 InitVelocity;
        public AABB Aabb;
        public float4 Color;
        public int Count;
    }
    
    static public void CreateSpawningRequest(EntityCommandBuffer.ParallelWriter ECB, int sortKey, SpawningRequestInfo info)
    {
        var entity = ECB.CreateEntity(sortKey);
        ECB.AddComponent(sortKey, entity, new SpawningRequest
        {
            Prefab = info.Prefab,
            Faction = info.Faction,
            InitVelocity = info.InitVelocity,
            Aabb = info.Aabb,
            Color = info.Color,
            Count = info.Count
        });
    }
}