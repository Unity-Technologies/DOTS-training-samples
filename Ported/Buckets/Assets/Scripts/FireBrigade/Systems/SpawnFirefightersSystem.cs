using FireBrigade.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace FireBrigade.Systems
{
    public class SpawnFirefightersSystem : SystemBase
    {
        private EntityCommandBufferSystem m_ECBSystem;
        private Random m_random;
        
        protected override void OnCreate()
        {
            m_ECBSystem = World.DefaultGameObjectInjectionWorld
                .GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
            m_random = new Random((uint)System.DateTime.Now.Millisecond);
        }

        protected override void OnUpdate()
        {
            var ecb = m_ECBSystem.CreateCommandBuffer();
            var random = m_random;
            Entities.ForEach((Entity entity, in FirefighterSpawner spawner) =>
            {
                var numGroups = random.NextInt(spawner.minGroups, spawner.maxGroups);
                Debug.Log($"Spawning Fire Brigade! Groups {spawner.minGroups} - {spawner.maxGroups}: {numGroups}");
                for (int i = 0; i < numGroups; i++)
                {
                    var x = random.NextFloat(-100f, 100f);
                    var z = random.NextFloat(-100f, 100f);
                    for (int j = 0; j < spawner.numPerGroup; j++)
                    {
                        var firefighterEntity = ecb.Instantiate(spawner.firefighterPrefab);
                        ecb.SetComponent(firefighterEntity, new GoalPosition
                        {
                            Value = new float3(x+j,0f,z)
                        });
                        ecb.SetComponent(firefighterEntity, new GroupIdentifier {Value = i});
                        ecb.SetComponent(firefighterEntity, new GroupIndex {Value = j});
                    }
                }
                ecb.DestroyEntity(entity);
            }).Schedule();
            m_ECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}