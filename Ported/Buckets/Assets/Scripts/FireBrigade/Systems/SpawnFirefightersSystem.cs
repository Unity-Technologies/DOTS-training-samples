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
            m_random = new Random((uint)UnityEngine.Random.Range(100,10000));
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
                    // debug
                    // pick a random water position
                    var waterPosition = random.NextFloat3() * random.NextFloat(-50f, 50f);
                    waterPosition.y = 0f;
                    // pick a random fire position
                    var firePosition = random.NextFloat3() * random.NextFloat(-50f, 50f);
                    firePosition.y = 0f;
                    for (int j = 0; j < spawner.numPerGroup; j++)
                    {
                        var firefighterEntity = ecb.Instantiate(spawner.firefighterPrefab);
                        ecb.SetComponent(firefighterEntity, new GroupIdentifier {Value = i});
                        ecb.SetComponent(firefighterEntity, new GroupIndex {Value = j});
                        ecb.AddComponent(firefighterEntity, new GroupCount {Value = spawner.numPerGroup});
                        ecb.AddComponent(firefighterEntity, new WaterPosition {Value = waterPosition});
                        ecb.AddComponent(firefighterEntity, new FirePosition {Value = firePosition});
                    }
                }
                ecb.DestroyEntity(entity);
            }).Schedule();
            m_ECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}