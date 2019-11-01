using System.Collections.Generic;
using Pathfinding;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;

// ReSharper disable once CheckNamespace
namespace GameAI
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class WorldCreatorSystem : ComponentSystem
    {
        public int2 WorldSize = new int2(100, 100);
        public int2 WorldSizeHalf => WorldSize / 2;

        public NativeArray<int> scoreArray;  // store in scoreArray[0]
        
        struct ExecuteOnceTag : IComponentData {}
        private EntityQuery m_executeOnce;

        Random rnd = new Random(0x12341fa);
        
        private EntityArchetype m_tile;
        private EntityArchetype m_plant;
        private EntityArchetype m_stone;
        private EntityArchetype m_score;    // singleton
        private EntityArchetype m_shop;

        public NativeHashMap<int2, Entity> hashMap;

        protected override void OnCreate()
        {
            ResetExecuteOnceTag(EntityManager);
            
            m_executeOnce = GetEntityQuery(ComponentType.ReadOnly<ExecuteOnceTag>());
            RequireForUpdate(m_executeOnce);

            m_tile = EntityManager.CreateArchetype(typeof(TilePositionRequest));
            m_plant = EntityManager.CreateArchetype(typeof(PlantPositionRequest));

            m_stone = EntityManager.CreateArchetype(typeof(StonePositionRequest));
            scoreArray = new NativeArray<int>(128, Allocator.Persistent);

            m_stone = EntityManager.CreateArchetype(typeof(StonePositionRequest));
            m_shop = EntityManager.CreateArchetype(typeof(ShopPositionRequest));
            
            hashMap = new NativeHashMap<int2, Entity>(1024, Allocator.Persistent);
        }
        
        protected override void OnUpdate()
        {
            Profiler.BeginSample("World Creation");

            var once = m_executeOnce.GetSingletonEntity();
            Assert.IsTrue(once != Entity.Null);
            EntityManager.DestroyEntity(once);

            // create TilePositionRequest's
            for (int x = 0; x < WorldSize.x; ++x)
            {
                for (int y = 0; y < WorldSize.y; ++y)
                {
                    var e = EntityManager.CreateEntity(m_tile);
                    EntityManager.SetComponentData(e, new TilePositionRequest {position = new int2(x, y)});
                }
            }

            int maxSize = math.max(WorldSize.x, WorldSize.y);
            
            for (int i = 0; i < maxSize*5; ++i)
            {
                int x = rnd.NextInt(WorldSize.x - 1);
                int y = rnd.NextInt(WorldSize.y - 1);
                int sx = rnd.NextInt(1, 5);
                int sy = rnd.NextInt(1, 5);

                var end_x = x + sx;
                var end_y = y + sy;
                
                end_x = math.min(end_x, WorldSize.x - 1);
                end_y = math.min(end_y, WorldSize.y - 1);

                sx = end_x - x;
                sy = end_y - y;

                for (int _x = x; _x <= end_x; _x++)
                    for (int _y = y; _y <= end_y; _y++)
                        if (hashMap.ContainsKey(new int2(_x, _y)))
                            goto WhoSaidGotoSuck;
                
                var e = EntityManager.CreateEntity(m_stone);
                EntityManager.SetComponentData(e, new StonePositionRequest {position = new int2(x, y), size = new int2(sx, sy)});
                
                for (int _x = x; _x <= end_x; _x++)
                    for (int _y = y; _y <= end_y; _y++)
                        hashMap.Add(new int2(_x, _y), e);

                WhoSaidGotoSuck: ;
            }

            for (int i = 0; i < maxSize*3; ++i)
            {
                var p = new int2(rnd.NextInt(WorldSize.x), rnd.NextInt(WorldSize.y));
                if (hashMap.ContainsKey(p) == false)
                {
                    var e = EntityManager.CreateEntity(m_plant);
                    EntityManager.SetComponentData(e, new PlantPositionRequest {position = p});
                }
            }

            for (;;)
            {
                var p = new int2(rnd.NextInt(WorldSize.x), rnd.NextInt(WorldSize.y));
                if (hashMap.ContainsKey(p) == false)
                {
                    var e = EntityManager.CreateEntity(m_shop);
                    EntityManager.SetComponentData(e, new ShopPositionRequest {position = p});

                    hashMap.Add(p, e);

                    break;
                }
            }
              
            for (;;)
            {
                var p = new int2(rnd.NextInt(WorldSize.x), rnd.NextInt(WorldSize.y));
                if (hashMap.ContainsKey(p) == false)
                {
                    var initialFarmerSpawnerEntity = EntityManager.CreateEntity(typeof(SpawnPointComponent), typeof(SpawnDroneTagComponent), typeof(SpawnFarmerTagComponent), typeof(InitialSpawnerTagComponent));
                    EntityManager.SetComponentData(initialFarmerSpawnerEntity, new SpawnPointComponent {MapSpawnPosition = p});
                    hashMap.Add(p, Entity.Null);

                    break;
                }
            }

            World.GetOrCreateSystem<PathfindingSystem>().PlantOrStoneChanged();
            Profiler.EndSample();
        }

        public static void ResetExecuteOnceTag(EntityManager mManager)
        {
            var q = mManager.CreateEntityQuery(ComponentType.ReadOnly<ExecuteOnceTag>());
            if (q.CalculateEntityCount() == 0)
            {
                var e = mManager.CreateEntity();
                mManager.AddComponent<ExecuteOnceTag>(e);
            }
        }

        protected override void OnDestroy()
        {
            scoreArray.Dispose();
            hashMap.Dispose();
            base.OnDestroy();
        }
    }
}