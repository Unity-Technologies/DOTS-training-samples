using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static Unity.Mathematics.math;

namespace GameAI
{
    public class ScoreSystem : JobComponentSystem
    {
        //public Entity Score; // singletonEntity
        private WorldCreatorSystem m_world;
        BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

        protected override void OnCreate()
        {
            m_world = World.GetOrCreateSystem<WorldCreatorSystem>();
            m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }
        
        protected unsafe override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var score = m_world.scoreArray;
            for (int i = 0; i < score.Length; i++)
            {
                score[i] = 0;
            }
            // sell plant +1
            var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            // how to know the plant has been sold?
            // the sub task sell plant
            
            var job = Entities
                .WithAll<AISubTaskSellPlant>()
                .WithNone<AISubTaskTagComplete>()
                .ForEach((Entity entity, int entityInQueryIndex, int nativeThreadIndex) => 
                {
                    ecb.AddComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    score[nativeThreadIndex] = score[nativeThreadIndex] + 1;
                }).Schedule(inputDeps);

            // smash rock +1
            var ecb2 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job2 = Entities
                .WithAll<AISubTaskTagClearRock>()
                .WithNone<AISubTaskTagComplete>()
                .ForEach((Entity entity, int entityInQueryIndex, int nativeThreadIndex) => 
                {
                    ecb2.AddComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    score[nativeThreadIndex] = score[nativeThreadIndex] + 1;
                }).Schedule(job);

            // spawn farmer -15
            // add SpawnFarmerTagComponent
            // add SpawnPointComponent
            var ecb3 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job3 = Entities
                .WithAll<TagShop>()
                .WithoutBurst()
                .ForEach((Entity e, int nativeThreadIndex, int entityInQueryIndex, in TilePositionable tile) =>
                {
                    // random chance it will spawn at this shop
                    var random = new Unity.Mathematics.Random();
                    var p = random.NextDouble(1.0);
                    if (p > 0.5)
                    {
                        // get shop position
                        var pos = tile.Position;
                        ecb3.AddComponent(entityInQueryIndex, e, new SpawnPointComponent{MapSpawnPosition = pos});
                        ecb3.AddComponent<SpawnFarmerTagComponent>(entityInQueryIndex, e);
                    }
                    
                    score[nativeThreadIndex] = score[nativeThreadIndex] - 15;
                }).Schedule(job2);
            
            // spawn drone -20
            // add SpawnDroneTagComponent
            // add SpawnPointComponent
            var ecb4 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job4 = Entities
                .WithAll<TagShop>()
                .WithoutBurst()
                .ForEach((Entity e, int nativeThreadIndex, int entityInQueryIndex, in TilePositionable tile) =>
                {
                    // random chance it will spawn at this shop
                    var random = new Unity.Mathematics.Random();
                    var p = random.NextDouble(1.0);
                    if (p < 0.5)
                    {
                        // get shop position
                        var pos = tile.Position;
                        ecb4.AddComponent(entityInQueryIndex, e, new SpawnPointComponent{MapSpawnPosition = pos});
                        ecb4.AddComponent<SpawnDroneTagComponent>(entityInQueryIndex, e);
                    }
                    
                    score[nativeThreadIndex] = score[nativeThreadIndex] - 20;
                }).Schedule(job3);

            m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job3);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job4);
            
            var handle = JobHandle.CombineDependencies(JobHandle.CombineDependencies(job, job2), JobHandle.CombineDependencies(job3, job4));
            handle.Complete();

            int score_tmp = 0;
            for (int i = 0; i < score.Length; i++)
            {
                score_tmp += score[i];
            }
            
            Debug.Log(score_tmp);

            return handle;
        }
    }
}