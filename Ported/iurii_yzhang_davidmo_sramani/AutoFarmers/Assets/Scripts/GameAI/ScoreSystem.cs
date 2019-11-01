using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
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
            var singletonScore = GetEntityQuery(typeof(CurrentScoreRequest)).GetSingletonEntity();
            var totalScore = EntityManager.GetComponentData<CurrentScoreRequest>(singletonScore).TotalScore;

            var score = m_world.scoreArray;
            
            // Just memset to zero
            UnsafeUtility.MemClear(NativeArrayUnsafeUtility.GetUnsafePtr(score), sizeof(int) * score.Length);
            var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            
            var job = Entities
                .WithAll<AISubTaskSellPlant>()
                .WithNone<AISubTaskTagComplete>()
                .ForEach((Entity entity, int entityInQueryIndex, int nativeThreadIndex) => 
                {
                    ecb.AddComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    score[nativeThreadIndex] = score[nativeThreadIndex] + 4;
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

            m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
            
            var handle = JobHandle.CombineDependencies(job, job2);
            handle.Complete();

            int score_tmp = 0;
            for (int i = 0; i < score.Length; i++)
            {
                score_tmp += score[i];
            }

            totalScore += score_tmp;
            var shopEntityArray = GetEntityQuery(typeof(TagShop)).ToEntityArray(Allocator.TempJob);
            
            foreach (var e in shopEntityArray)
            {
                if (totalScore >= 10)
                {
                    EntityManager.AddComponent<SpawnFarmerTagComponent>(e);
                    totalScore -= 10;
                }
                if (totalScore >= 20)
                {
                    EntityManager.AddComponent<SpawnDroneTagComponent>(e);
                    totalScore -= 20;
                }
            }
            
            EntityManager.SetComponentData<CurrentScoreRequest>(singletonScore, new CurrentScoreRequest{TotalScore = totalScore});
            shopEntityArray.Dispose();

            return inputDeps;
        }
    }
}