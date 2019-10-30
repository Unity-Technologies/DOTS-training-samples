using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;
using Random = Unity.Mathematics.Random;

namespace GameAI
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class AiTaskSelectSys : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            var ecb1 = ecbSystem.CreateCommandBuffer().ToConcurrent();
            var rnd = Time.time;

            var groundSelectorHandle = Entities
                .WithAll<AITaskTagNone, FarmerAITag>()
//            .WithoutBurst()
                .ForEach((int nativeThreadIndex, Entity e) =>
                {
                    var seed = nativeThreadIndex * (int) rnd * 7;
                    var randomNum = new Random((uint) seed);

                    ecb1.RemoveComponent(nativeThreadIndex, e, typeof(AITaskTagNone));
                    switch (randomNum.NextInt(4))
                    {
                        default:
                            ecb1.AddComponent(nativeThreadIndex, e, new AITaskTagClearRock());
                            break;
                        case 1:
                            ecb1.AddComponent(nativeThreadIndex, e, new AITaskTagTill());
                            break;
                        case 2:
                            ecb1.AddComponent(nativeThreadIndex, e, new AITaskTagPlant());
                            break;
                        case 3:
                            ecb1.AddComponent(nativeThreadIndex, e, new AITaskTagCollect());
                            break;
                    }
                }).Schedule(inputDependencies);

            var ecb2 = ecbSystem.CreateCommandBuffer().ToConcurrent();
            var airSelectorHandle = Entities
                .WithNone<FarmerAITag>()
                .WithAll<AITaskTagNone>()
//            .WithoutBurst()
                .ForEach((int nativeThreadIndex, Entity e) =>
                {
                    ecb1.RemoveComponent(nativeThreadIndex, e, typeof(AITaskTagNone));
                    ecb1.AddComponent(nativeThreadIndex, e, new AITaskTagCollect());
                }).Schedule(inputDependencies);

            ecbSystem.AddJobHandleForProducer(groundSelectorHandle);
            ecbSystem.AddJobHandleForProducer(airSelectorHandle);

            return JobHandle.CombineDependencies(groundSelectorHandle, airSelectorHandle);
        }
    }
}