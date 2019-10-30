using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameAI
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class WorldCreatorSystem : ComponentSystem
    {
        [BurstCompile]
        struct SpawnFarmer : IJobParallelFor
        {
            public EntityCommandBuffer.Concurrent ec;
            public Entity Prefab;
            public Vector2Int Position;
        
            public void Execute(int index)
            {
                var e = ec.Instantiate(index, Prefab);
                ec.SetComponent(index, e, new FarmerComponent() { Position = Position });
            }
        }

        [BurstCompile]
        struct SpawnRock : IJobParallelFor
        {
            public EntityCommandBuffer.Concurrent ec;
            public Entity prefab;
            public Vector2Int position;
            public Unity.Mathematics.Random random;

            public void Execute(int index)
            {
                var e = ec.Instantiate(index, prefab);
                var p = random.NextFloat();
                ec.SetComponent(index, e, new RockComponent() { Position = position, Size = p });
            }
        }
        
        [BurstCompile]
        struct SpawnShop : IJobParallelFor
        {
            public EntityCommandBuffer.Concurrent ec;
            public Entity prefab;
            public Vector2Int position;
        
            public void Execute(int index)
            {
                var e = ec.Instantiate(index, prefab);
                ec.SetComponent(index, e, new ShopComponent() { Position = position });
            }
        }
        
        private Entity m_farmerEntity;
        private Entity m_rockEntity;
        private Entity m_shopEntity;
        
        protected override void OnCreate()
        {
            {
                var farmerPrefabArchetype =
                    EntityManager.CreateArchetype(typeof(Translation), typeof(LocalToWorld), typeof(FarmerComponent));

                m_farmerEntity = EntityManager.CreateEntity(farmerPrefabArchetype);
                EntityManager.SetSharedComponentData(m_farmerEntity, new RenderMesh {
                                                                                        mesh = PrefabUtility
                                                                                               .LoadPrefabContents("cubeprefab")
                                                                                               .GetComponent<MeshFilter
                                                                                               >()
                                                                                               .mesh,
                                                                                        material = PrefabUtility
                                                                                                   .LoadPrefabContents("cubeprefab")
                                                                                                   .GetComponent<
                                                                                                       Renderer>()
                                                                                                   .sharedMaterial,
                                                                                        castShadows =
                                                                                            ShadowCastingMode.On,
                                                                                        receiveShadows = true
                                                                                    });
            }
            {
                var rockPrefabArchetype =
                    EntityManager.CreateArchetype(typeof(Translation), typeof(LocalToWorld), typeof(RockComponent));

                m_rockEntity = EntityManager.CreateEntity(rockPrefabArchetype);
                EntityManager.SetSharedComponentData(m_rockEntity, new RenderMesh {
                                                                                      mesh = PrefabUtility
                                                                                             .LoadPrefabContents("sphereprefab")
                                                                                             .GetComponent<MeshFilter>()
                                                                                             .mesh,
                                                                                      material = PrefabUtility
                                                                                                 .LoadPrefabContents("sphereprefab")
                                                                                                 .GetComponent<Renderer
                                                                                                 >()
                                                                                                 .sharedMaterial,
                                                                                      castShadows =
                                                                                          ShadowCastingMode.On,
                                                                                      receiveShadows = true
                                                                                  });
            }
            {
                var shopPrefabArchetype =
                    EntityManager.CreateArchetype(typeof(Translation), typeof(LocalToWorld), typeof(ShopComponent));

                m_shopEntity = EntityManager.CreateEntity(shopPrefabArchetype);
                EntityManager.SetSharedComponentData(m_shopEntity, new RenderMesh {
                                                                                      mesh = PrefabUtility
                                                                                             .LoadPrefabContents("cylinderprefab")
                                                                                             .GetComponent<MeshFilter>()
                                                                                             .mesh,
                                                                                      material = PrefabUtility
                                                                                                 .LoadPrefabContents("cylinderprefab")
                                                                                                 .GetComponent<Renderer
                                                                                                 >()
                                                                                                 .sharedMaterial,
                                                                                      castShadows =
                                                                                          ShadowCastingMode.On,
                                                                                      receiveShadows = true
                                                                                  });
            }
        }

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            var farmers = new SpawnFarmer {
                                                ec = ecb.ToConcurrent(),
                                                Prefab = m_farmerEntity,
                                                Position = Vector2Int.zero
                                            };
            var rocks = new SpawnRock {
                                      ec = ecb.ToConcurrent(),
                                      prefab = m_farmerEntity,
                                      position = Vector2Int.zero,
                                      random = new Unity.Mathematics.Random(0xBAD5EED)
                                  };
            var shops = new SpawnShop {
                                      ec = ecb.ToConcurrent(),
                                      prefab = m_shopEntity,
                                      position = Vector2Int.zero
                                  };

            farmers.Schedule(1, 1);
            rocks.Schedule(2000, 64);
            shops.Schedule(1000, 64).Complete();
            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}