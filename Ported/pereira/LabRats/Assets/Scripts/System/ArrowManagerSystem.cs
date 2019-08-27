using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public class ArrowManagerSystem : JobComponentSystem
{
    private EntityQuery m_ArrowSpawerQuery;
    private EntityQuery m_BoardQuery;
    private EntityQuery m_ArrowsQuery;
    private EntityQuery m_ArrowsDestroyedQuery;
    EntityCommandBufferSystem m_EntityCommandBufferSystem;
        
    protected override void OnCreate()
    {
        m_EntityCommandBufferSystem = World.Active.GetOrCreateSystem<LbSimulationBarrier>();
        m_ArrowSpawerQuery = GetEntityQuery(typeof(LbArrowSpawner));
        m_BoardQuery = GetEntityQuery(typeof(LbBoard), typeof(LbArrowDirectionMap));
        m_ArrowsQuery = GetEntityQuery(typeof(LbArrow));
        m_ArrowsDestroyedQuery = GetEntityQuery(typeof(LbArrow),typeof(LbDestroy));
    }

    struct SpawnArrow : IJob
    {        
        [DeallocateOnJobCompletion]public NativeArray<Entity> Entities;
        [DeallocateOnJobCompletion]public NativeArray<LbArrowSpawner> ArrowSpawners;
        public EntityCommandBuffer CommandBuffer;

        public void Execute()
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                var arrowSpawner = ArrowSpawners[i];
                var rotationDegrees = ArrowSpawners[i].Direction * 90;
                var instance = CommandBuffer.Instantiate(ArrowSpawners[i].Prefab);
                    
                ArrowSpawners[i] = arrowSpawner;
                CommandBuffer.AddComponent(instance, new LbArrow {Location = new int2((int)ArrowSpawners[i].Location.x,(int)ArrowSpawners[i].Location.z),Direction = ArrowSpawners[i].Direction});
                CommandBuffer.AddComponent( instance, new LbLifetime { Value = 10f});
                CommandBuffer.SetComponent( instance, new Translation{Value = new float3(ArrowSpawners[i].Location.x,0.85f,ArrowSpawners[i].Location.z)});
                CommandBuffer.SetComponent( instance, new Rotation{Value = quaternion.EulerXYZ(math.radians(90),math.radians(rotationDegrees),math.radians(0))});
                CommandBuffer.DestroyEntity(Entities[i]);
            }
        }
    }

    [BurstCompile]
    struct DeleteDestroyedFromArroMap : IJob
    {
        [DeallocateOnJobCompletion]public NativeArray<Entity> Entities;
        public NativeArray<LbArrowDirectionMap> ArrowDirectionMap;
        [DeallocateOnJobCompletion]public NativeArray<LbArrow> LbArrows;
        public int Size;
        public void Execute()
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                var arrowMapIndex = (int)(LbArrows[i].Location.y * Size +
                                          LbArrows[i].Location.x);
                var currentWord = ArrowDirectionMap[arrowMapIndex].Value;
                currentWord &= 0x0;
                ArrowDirectionMap[arrowMapIndex] = new LbArrowDirectionMap(){ Value = currentWord};
            }
        }
    }

    [BurstCompile]
    struct WriteArroMap : IJob
    {
        [DeallocateOnJobCompletion]public NativeArray<Entity> Entities;
         public NativeArray<LbArrowDirectionMap> ArrowDirectionMap;
        [DeallocateOnJobCompletion]public NativeArray<LbArrow> LbArrows;
        public int Size;
        
        public void Execute()
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                var arrowMapIndex = (int)(LbArrows[i].Location.y * Size +
                                          LbArrows[i].Location.x);
                var currentWord = ArrowDirectionMap[arrowMapIndex].Value;
                currentWord |= 0x10;
                currentWord |= LbArrows[i].Direction;
                ArrowDirectionMap[arrowMapIndex] = new LbArrowDirectionMap(){ Value = currentWord};
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var board = m_BoardQuery.GetSingleton<LbBoard>();
        var entitiesSpawners = m_ArrowSpawerQuery.ToEntityArray(Allocator.TempJob);
        var entitiesArrows = m_ArrowsQuery.ToEntityArray(Allocator.TempJob);
        var arrowSpawners = m_ArrowSpawerQuery.ToComponentDataArray<LbArrowSpawner>(Allocator.TempJob);
        var boardEntity = m_BoardQuery.GetSingletonEntity();
        var arrowsQuery = m_ArrowsQuery.ToComponentDataArray<LbArrow>(Allocator.TempJob);
        var bufferLookup = GetBufferFromEntity<LbArrowDirectionMap>();
        var buffer = bufferLookup[boardEntity];
        var bufferArray = buffer.AsNativeArray();
        var commandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer();
        
        var handle = new MemsetNativeArray<LbArrowDirectionMap>()
        {
            Source = bufferArray,
            Value = new LbArrowDirectionMap()
        }.Schedule(bufferArray.Length, bufferArray.Length, inputDeps);
        
         handle = new SpawnArrow
        {
            Entities = entitiesSpawners,
            ArrowSpawners = arrowSpawners,
            CommandBuffer = commandBuffer
        }.Schedule(handle);
        
        
         handle = new WriteArroMap
        {
            Size = board.SizeY,
            Entities = entitiesArrows,
            LbArrows = arrowsQuery,
            ArrowDirectionMap = bufferArray,
        }.Schedule(handle);
        
        m_EntityCommandBufferSystem.AddJobHandleForProducer(handle);

        return handle;
    }
}
