using Enums;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;
using static ConfigAuthoring;

namespace Systems
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial struct BotSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ConfigAuthoring.Config>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var config = SystemAPI.GetSingleton<ConfigAuthoring.Config>();

            CreateOmniBots(ref state, ref config);
        }

        public static void CreateOmniBots(ref SystemState state, ref ConfigAuthoring.Config config)
        {
            var rand = new Random(234);
            for (int i = 0; i < config.numOmnibots; i++)
            {
                var omniBot = state.EntityManager.Instantiate(config.botPrefab);
                var x = rand.NextFloat(0f, config.simulationWidth);
                var z = rand.NextFloat(0f, config.simulationDepth);

                state.EntityManager.SetComponentData(omniBot,
                    LocalTransform.FromPosition(x, 0.5f, z));

                state.EntityManager.SetComponentData(omniBot,
                    new BotCommand() { Value = Enums.BotAction.GET_BUCKET });

                state.EntityManager.SetComponentData(omniBot,
                    new URPMaterialPropertyBaseColor() { Value = config.botOmniColor });

                state.EntityManager.SetComponentData(omniBot,
                    new DecisionTimer { value = 0.0f });

                state.EntityManager.SetComponentData(omniBot,
                    new TargetBucket { value = Entity.Null });

                state.EntityManager.SetComponentData(omniBot,
                    new TargetWater { value = Entity.Null });

                state.EntityManager.SetComponentData(omniBot,
                    new TargetFlame { value = Entity.Null });

                state.EntityManager.SetComponentData(omniBot,
                    new ArriveThreshold { value = 1.0f });
                
                state.EntityManager.SetComponentData(omniBot, new BotCommand{Value = BotAction.GET_BUCKET});
            }
        }
    }
}
