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

        
        struct ExecuteOnceTag : IComponentData {}
        private EntityQuery m_executeOnce;

        Random rnd = new Random(0x12341fa);
        
        private EntityArchetype m_tile;
        private EntityArchetype m_plant;
        private EntityArchetype m_store;

        protected override void OnCreate()
        {
            ResetExecuteOnceTag(EntityManager);
            
            m_executeOnce = GetEntityQuery(ComponentType.ReadOnly<ExecuteOnceTag>());
            RequireForUpdate(m_executeOnce);

            m_tile = EntityManager.CreateArchetype(typeof(TilePositionRequest));
            m_plant = EntityManager.CreateArchetype(typeof(PlantPositionRequest));
            m_store = EntityManager.CreateArchetype(typeof(StonePositionRequest));
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
            
            for (int i = 0; i < maxSize*2; ++i)
            {
                int x = rnd.NextInt(WorldSize.x);
                int y = rnd.NextInt(WorldSize.y);
                
                var e = EntityManager.CreateEntity(m_plant);
                EntityManager.SetComponentData(e, new PlantPositionRequest {position = new int2(x, y)});
            }
            
            for (int i = 0; i < maxSize*3; ++i)
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
                
                // TODO: check for other objects
                
                var e = EntityManager.CreateEntity(m_store);
                EntityManager.SetComponentData(e, new StonePositionRequest {position = new int2(x, y), size = new int2(sx, sy)});
            }
            
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
    }
}