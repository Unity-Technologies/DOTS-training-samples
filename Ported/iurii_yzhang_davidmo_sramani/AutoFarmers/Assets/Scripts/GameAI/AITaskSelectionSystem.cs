using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
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
            var rnd = DateTime.Now.Ticks;
            var randomNum = new Random();

            var groundSelectorHandle = Entities
                .WithAll<AITagTaskNone, FarmerAITag>()
//            .WithoutBurst()
                .ForEach((int nativeThreadIndex, Entity e) =>
                {
                    var seed = nativeThreadIndex + e.Index + e.Version + rnd;
                    randomNum.InitState((uint)seed);

                    ecb1.RemoveComponent<AITagTaskNone>(nativeThreadIndex, e);
                    switch (randomNum.NextInt(0,3))
                    {
                        default:
                            ecb1.AddComponent<AITagTaskClearRock>(nativeThreadIndex, e);
                            break;
                        case 1:
                            ecb1.AddComponent<AITagTaskTill>(nativeThreadIndex, e);
                            break;
                        case 2:
                            ecb1.AddComponent<AITagTaskDeliver>(nativeThreadIndex, e);
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
                    ecb2.RemoveComponent<AITagTaskNone>(nativeThreadIndex, e);
                    ecb2.AddComponent<AITagTaskDeliver>(nativeThreadIndex, e);
                }).Schedule(inputDependencies);

            ecbSystem.AddJobHandleForProducer(groundSelectorHandle);
            ecbSystem.AddJobHandleForProducer(airSelectorHandle);

            return JobHandle.CombineDependencies(groundSelectorHandle, airSelectorHandle);
        }
    }
}