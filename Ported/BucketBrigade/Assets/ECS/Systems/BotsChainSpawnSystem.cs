using System.Security.Cryptography;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public struct BotChainElementData : IBufferElementData
{
    public Entity passerFull;
    public Entity passerEmpty;
}

public class BotsChainSpawnSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameConfigComponent>();
    }

    protected override void OnUpdate()
    {
        // Run just once, unless reset elsewhere.
        Enabled = false;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var config = GetSingleton<GameConfigComponent>();
        var chainsCount = config.ChainsCount;
        var chainSize = config.ChainSize;
        var simSize = config.SimulationSize;
        var scooperColor = config.ScooperBotColor;
        var throwerColor = config.ThrowerBotColor;
        var passerFullColor = config.PasserFullBotColor;
        var passerEmptyColor = config.PasserEmptyBotColor;
        var botPrefab = config.BotPrefab;
        var scooperPrefab = config.ScooperPrefab;
        var passerFullPrefab = config.PasserFullPrefab;
        var passerEmptyPrefab = config.PasserEmptyPrefab; //To Do: combine the 2 and add different components on demand

        var rand = new Unity.Mathematics.Random();
        rand.InitState();

        for (var i = 0; i < chainsCount; ++i)
        {
            var scooper = CreateBot(ref ecb, ref rand, simSize, ref scooperPrefab, scooperColor);
            var thrower = CreateBot(ref ecb, ref rand, simSize, ref botPrefab, throwerColor);

            var chain = ecb.CreateEntity();
            var chainComp = new BotsChainComponent()
            {
                scooper = scooper,
                thrower = thrower
            };
            ecb.AddComponent(chain, chainComp);

            var botsBuffer = ecb.AddBuffer<BotChainElementData>(chain);
            botsBuffer.ResizeUninitialized(chainSize);
            for (int b = 0; b < chainSize; ++b)
            {
                var passerFull = CreateBot(ref ecb, ref rand, simSize, ref passerFullPrefab, passerFullColor);
                var passerEmpty = CreateBot(ref ecb, ref rand, simSize, ref passerEmptyPrefab, passerEmptyColor);
                botsBuffer[b] = new BotChainElementData() {passerFull = passerFull, passerEmpty = passerEmpty};
            }
        }

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }

    Entity CreateBot(ref EntityCommandBuffer ecb, ref Random rnd, int simSize, ref Entity prefab, Color color)
    {
        var bot = ecb.Instantiate(prefab);
        ecb.SetComponent(bot, new Translation() {Value = new float3(rnd.NextFloat() * simSize, 0.0f, rnd.NextFloat() * simSize)});
        ecb.SetComponent(bot, new URPMaterialPropertyBaseColor() {Value = ColorToFloat4.Cast(color)});
        ecb.SetComponent(bot, new TargetLocationComponent() {location = float2.zero});
        ecb.AddComponent<TargetBucket>(bot);
        ecb.AddComponent<CarriedBucket>(bot);
        
        return bot;
    }
}

public static class ColorToFloat4
{
    public static float4 Cast(Color color)
    {
        return new float4(color.r, color.g, color.b, 1.0f);
    }
}