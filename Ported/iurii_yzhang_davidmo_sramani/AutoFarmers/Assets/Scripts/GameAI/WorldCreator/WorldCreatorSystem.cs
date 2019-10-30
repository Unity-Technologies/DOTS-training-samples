using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;

namespace GameAI
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class WorldCreatorSystem : ComponentSystem
    {
        public const int worldSizeN = 100;
        public const int worldSizeM = 100;
        
        private Entity m_executeOnceEntity;
        struct ExecuteOnceTag : IComponentData {}

        Random rnd = new Random(23);
        
        private EntityArchetype m_tile;
        private EntityArchetype m_plant;
        private EntityArchetype m_store;

        protected override void OnCreate()
        {
            m_executeOnceEntity = EntityManager.CreateEntity();
            EntityManager.AddComponent<ExecuteOnceTag>(m_executeOnceEntity);

            var query = GetEntityQuery(ComponentType.ReadOnly<ExecuteOnceTag>());
            RequireForUpdate(query);

            m_tile = EntityManager.CreateArchetype(typeof(TilePositionRequest));
            m_plant = EntityManager.CreateArchetype(typeof(PlantPositionRequest));
            m_store = EntityManager.CreateArchetype(typeof(StonePositionRequest));
        }

        protected override void OnUpdate()
        {
            EntityManager.DestroyEntity(m_executeOnceEntity);
            m_executeOnceEntity = Entity.Null;
            
            // create TilePositionRequest's
            for (int x = 0; x < worldSizeN; ++x)
            {
                for (int y = 0; y < worldSizeM; ++y)
                {
                    var e = EntityManager.CreateEntity(m_tile);
                    EntityManager.SetComponentData(e, new TilePositionRequest {position = new int2(x, y)});
                }
            }

            for (int i = 0; i < worldSizeN*2; ++i)
            {
                int x = rnd.NextInt(worldSizeN);
                int y = rnd.NextInt(worldSizeM);
                
                var e = EntityManager.CreateEntity(m_plant);
                EntityManager.SetComponentData(e, new PlantPositionRequest {position = new int2(x, y)});
            }
            
            for (int i = 0; i < worldSizeN*3; ++i)
            {
                int x = rnd.NextInt(worldSizeN);
                int y = rnd.NextInt(worldSizeM);
                int sx = rnd.NextInt(1, 5);
                int sy = rnd.NextInt(1, 5);

                // TODO: check for other objects
                
                var e = EntityManager.CreateEntity(m_store);
                EntityManager.SetComponentData(e, new StonePositionRequest {position = new int2(x, y), size = new int2(sx, sy)});
            }
            
            Debug.Log("WorldCreatorSystem");
        }
    }
}