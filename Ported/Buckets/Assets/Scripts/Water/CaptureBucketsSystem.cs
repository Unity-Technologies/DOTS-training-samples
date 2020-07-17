using FireBrigade.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.SceneManagement;

namespace Water
{
    public class CaptureBucketsSystem : SystemBase
    {
        private EntityCommandBufferSystem m_ECBSystem;
        private EntityQuery bucketsQuery;
        
        protected override void OnCreate()
        {
            m_ECBSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            bucketsQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(BucketTag), typeof(Attached));
        }
        
        protected override void OnUpdate()
        {
            var buckets = bucketsQuery.ToEntityArray(Allocator.TempJob);
            var ecb = m_ECBSystem.CreateCommandBuffer();

            Entities
                .WithDeallocateOnJobCompletion(buckets)
                .WithAll<WellTag,InUse>()
                .ForEach((Entity entity, in LocalToWorld transform) =>
                {
                    for (int i = 0; i < buckets.Length; i++)
                    {
                        var bucketPosition = GetComponent<Translation>(buckets[i]).Value;
                        if (math.distance(bucketPosition, transform.Position) < 1f)
                        {
                            if (HasComponent<Attached>(buckets[i]))
                            {
                                var oldAttach = GetComponent<Attached>(buckets[i]);
                                ecb.RemoveComponent<HeldBucket>(oldAttach.Value);
                            }
                            var attachToWell = new Attached();
                            attachToWell.Offset = new float3(0f, 0.5f, 0f);
                            attachToWell.Value = entity;
                            if (HasComponent<Attached>(buckets[i]))
                            {
                                SetComponent(buckets[i], attachToWell);
                            }
                            else
                            {
                                ecb.AddComponent(buckets[i], attachToWell);
                            }
                            // ecb.AddComponent<InUse>(buckets[i]);
                        }
                    }

                }).Schedule();
            m_ECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}