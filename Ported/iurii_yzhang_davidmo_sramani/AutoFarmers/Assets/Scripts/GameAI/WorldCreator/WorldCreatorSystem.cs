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
using UnityEngine.Experimental.Rendering;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;
using SysInfo = UnityEngine.SystemInfo; 

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
        
        private Entity m_tile;
        private Entity m_plant;
        private Entity m_stone;
        private EntityArchetype m_score;    // singleton
        private Entity m_shop;

        public NativeHashMap<int2, Entity> hashMap;

        protected override void OnCreate()
        {
            var ru = RenderingUnity.instance;
            ResetExecuteOnceTag(EntityManager);
            
            m_executeOnce = GetEntityQuery(ComponentType.ReadOnly<ExecuteOnceTag>());
            RequireForUpdate(m_executeOnce);

            m_tile = ru.CreateGround(EntityManager);
            m_plant = ru.CreatePlant(EntityManager);
            
            m_score = EntityManager.CreateArchetype(typeof(CurrentScoreRequest));
            scoreArray = new NativeArray<int>(SysInfo.processorCount * 4, Allocator.Persistent);
            
            m_stone = ru.CreateStone(EntityManager);
            m_shop = ru.CreateStore(EntityManager);
            
            hashMap = new NativeHashMap<int2, Entity>(1024, Allocator.Persistent);
        }
        
        protected override void OnUpdate()
        {
            Profiler.BeginSample("World Creation");

            var once = m_executeOnce.GetSingletonEntity();
            Assert.IsTrue(once != Entity.Null);
            EntityManager.DestroyEntity(once);

            var worldSizeHalf = World.GetOrCreateSystem<WorldCreatorSystem>().WorldSizeHalf;
            var halfSizeOffset = new float3(RenderingUnity.scale * 0.5f, 0, RenderingUnity.scale * 0.5f);



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
                
                var e = EntityManager.Instantiate(m_stone);
                int2 position = new int2(x, y);
                int2 size = new int2(sx, sy);
                
                var wpos = RenderingUnity.Tile2WorldPosition(position, worldSizeHalf); // min pos
                var wsize = RenderingUnity.Tile2WorldSize(size) - 0.1f;
                var wposCenter = wpos + wsize * 0.5f - halfSizeOffset;

                EntityManager.SetComponentData(e, new Translation() {Value = wposCenter});
                EntityManager.SetComponentData(e, new NonUniformScale() {Value = wsize});
                EntityManager.SetComponentData(e, new TilePositionable() {Position = position});
                EntityManager.SetComponentData(e, new RockComponent() {Size = new int2(sx, sy)});

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
                    var e = EntityManager.Instantiate(m_plant);
                    EntityManager.SetComponentData(e, new TilePositionable() {Position = p});
                    var wpos = RenderingUnity.Tile2WorldPosition(p, worldSizeHalf);
                    EntityManager.SetComponentData(e, new Translation() {Value = wpos});
                    hashMap.Add(p, e);
                }
            }

            for (;;)
            {
                var p = new int2(rnd.NextInt(WorldSize.x), rnd.NextInt(WorldSize.y));
                if (hashMap.ContainsKey(p) == false)
                {
                    var e = EntityManager.Instantiate(m_shop);
                    EntityManager.SetComponentData(e, new TilePositionable() {Position = p});

                    var wpos = RenderingUnity.Tile2WorldPosition(p, worldSizeHalf);
                    EntityManager.SetComponentData(e, new Translation() {Value = wpos});
                    EntityManager.SetComponentData(e, new SpawnPointComponent() {MapSpawnPosition = p});
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
            
            // create TilePositionRequest's
            for (int x = 0; x < WorldSize.x; ++x)
            {
                for (int y = 0; y < WorldSize.y; ++y)
                {
                    var e = EntityManager.Instantiate(m_tile);
                    var p = new int2(x, y);
                    EntityManager.SetComponentData(e, new TilePositionable() {Position = p});
                    var wpos = RenderingUnity.Tile2WorldPosition(p, worldSizeHalf) - new float3(0, 0.5f, 0);
                    EntityManager.SetComponentData(e, new LocalToWorld() {Value = float4x4.TRS(wpos, quaternion.identity, new float3(0.2f, 1, 0.2f))});
                    
                    if (!hashMap.ContainsKey(p)) {
                        hashMap[p] = e;
                    }
                }
            }
            
            var es = EntityManager.CreateEntity(m_score);
            EntityManager.SetComponentData(es, new CurrentScoreRequest {TotalScore = 0});

            World.GetOrCreateSystem<PathfindingSystem>().DistFieldDirty();
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