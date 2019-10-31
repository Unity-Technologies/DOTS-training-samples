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
    public class AiTaskSelectSys : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            var ecb1 = ecbSystem.CreateCommandBuffer().ToConcurrent();
            var rnd = Time.time;

            var groundSelectorHandle = Entities
                .WithAll<AITagTaskNone, FarmerAITag>()
//            .WithoutBurst()
                .ForEach((int nativeThreadIndex, Entity e) =>
                {
                    var seed = nativeThreadIndex * (int) rnd * 7;
                    var randomNum = new Random((uint) seed);

                    ecb1.RemoveComponent(nativeThreadIndex, e, typeof(AITagTaskNone));
                    switch (randomNum.NextInt(4))
                    {
                        default:
                            ecb1.AddComponent(nativeThreadIndex, e, new AITagTaskClearRock());
                            break;
                        case 1:
                            ecb1.AddComponent(nativeThreadIndex, e, new AITagTaskTill());
                            break;
                        case 2:
                            ecb1.AddComponent(nativeThreadIndex, e, new AITagTaskPlant());
                            break;
                        case 3:
                            ecb1.AddComponent(nativeThreadIndex, e, new AITagTaskCollect());
                            break;
                    }
                }).Schedule(inputDependencies);

            var ecb2 = ecbSystem.CreateCommandBuffer().ToConcurrent();
            var airSelectorHandle = Entities
                .WithNone<FarmerAITag>()
                .WithAll<AITagTaskNone>()
//            .WithoutBurst()
                .ForEach((int nativeThreadIndex, Entity e) =>
                {
                    ecb1.RemoveComponent(nativeThreadIndex, e, typeof(AITagTaskNone));
                    ecb1.AddComponent(nativeThreadIndex, e, new AITagTaskCollect());
                }).Schedule(inputDependencies);

            ecbSystem.AddJobHandleForProducer(groundSelectorHandle);
            ecbSystem.AddJobHandleForProducer(airSelectorHandle);

            return JobHandle.CombineDependencies(groundSelectorHandle, airSelectorHandle, inputDependencies);
        }
    }
}