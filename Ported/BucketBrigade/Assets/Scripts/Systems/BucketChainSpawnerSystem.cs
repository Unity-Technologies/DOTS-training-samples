using Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateAfter(typeof(OmniBotSpawnerSystem))]
    public partial struct BucketChainSpawnerSystem : ISystem
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

            for (int i = 0; i < config.totalBucketChains; i++)
            {
                CreateBucketChain(ref state, ref config);
            }
            
        }
        
        private void CreateBucketChain(ref SystemState state, ref ConfigAuthoring.Config config)
        {
            var bucketChain = state.EntityManager.CreateEntity();
            var rand = new Random(234);
            //scooper
            var scooperBot = state.EntityManager.Instantiate(config.botPrefab);
            state.EntityManager.AddComponent<BotScooper>(scooperBot);
            state.EntityManager.AddComponent<TargetWater>(scooperBot);
            state.EntityManager.AddComponent<LocationPickup>(scooperBot);
            state.EntityManager.AddComponent<LocationDropoff>(scooperBot);
            var x = rand.NextFloat(0f, config.simulationWidth);
            var z = rand.NextFloat(0f, config.simulationDepth);
            state.EntityManager.SetComponentData(scooperBot,
                LocalTransform.FromPosition(x, 0.5f, z));

            //thrower
            var throwerBot = state.EntityManager.Instantiate(config.botPrefab);
            state.EntityManager.AddComponent<BotThrower>(throwerBot);
            state.EntityManager.AddComponent<TargetFlame>(throwerBot);
            state.EntityManager.AddComponent<LocationPickup>(throwerBot);
            state.EntityManager.AddComponent<LocationDropoff>(throwerBot);
            x = rand.NextFloat(0f, config.simulationWidth);
            z = rand.NextFloat(0f, config.simulationDepth);
            state.EntityManager.SetComponentData(throwerBot,
                LocalTransform.FromPosition(x, 0.5f, z));
            
            state.EntityManager.AddComponentData<BucketChain>(bucketChain, new BucketChain{Scooper = scooperBot, Thrower = throwerBot});

            //NativeArray<Entity> botPassersEmpty = new NativeArray<Entity>(config.totalPassersEmpty, Allocator.Temp);
            var botPassersEmpty = CollectionHelper.CreateNativeArray<Entity>(config.totalPassersEmpty, state.WorldUpdateAllocator);

            //chain of pass and empty bots
            for (int i = 0; i < config.totalPassersEmpty; i++)
            {
                var tempBot = state.EntityManager.Instantiate(config.botPrefab);
                state.EntityManager.AddComponent<BotPasser>(tempBot);
                state.EntityManager.AddComponent<BucketProvider>(tempBot);
                x = rand.NextFloat(0f, config.simulationWidth);
                z = rand.NextFloat(0f, config.simulationDepth);
                state.EntityManager.SetComponentData(tempBot,
                    LocalTransform.FromPosition(x, 0.5f, z));
                
                botPassersEmpty[i] = tempBot;

                if (i == 0)
                {
                    state.EntityManager.SetComponentData(tempBot,
                        new BucketProvider{Value = throwerBot});
                }
                else
                {
                    state.EntityManager.SetComponentData(tempBot,
                        new BucketProvider{Value = botPassersEmpty[i-1]});
                }
            }

            var emptyPasserBuffer = state.EntityManager.AddBuffer<PasserEmpty>(bucketChain);
            emptyPasserBuffer.Length = config.totalPassersEmpty;
            for (int c = 0; c < config.totalPassersEmpty; c++)
            {
                emptyPasserBuffer.Add(new PasserEmpty{Value = botPassersEmpty[c]});
            }

            //NativeArray<Entity> botPassersFull = new NativeArray<Entity>(config.totalPassersFull, Allocator.Temp);
            var botPassersFull = CollectionHelper.CreateNativeArray<Entity>(config.totalPassersFull, state.WorldUpdateAllocator);
            
            for (int i = 0; i < config.totalPassersFull; i++)
            {
                var tempBot = state.EntityManager.Instantiate(config.botPrefab);
                state.EntityManager.AddComponent<BotPasser>(tempBot);
                state.EntityManager.AddComponent<BucketProvider>(tempBot);
                x = rand.NextFloat(0f, config.simulationWidth);
                z = rand.NextFloat(0f, config.simulationDepth);
                state.EntityManager.SetComponentData(tempBot,
                    LocalTransform.FromPosition(x, 0.5f, z));
                
                botPassersFull[i] = tempBot;
                
                if (i == 0)
                {
                    state.EntityManager.SetComponentData(tempBot,
                        new BucketProvider{Value = scooperBot});
                }
                else
                {
                    state.EntityManager.SetComponentData(tempBot,
                        new BucketProvider{Value = botPassersFull[i-1]});
                }
            }
            var fullPasserBuffer = state.EntityManager.AddBuffer<PasserFull>(bucketChain);
            fullPasserBuffer.Length = config.totalPassersFull;
            for (int c = 0; c < config.totalPassersFull; c++)
            {
                fullPasserBuffer.Add(new PasserFull{Value = botPassersFull[c]});
            }

            //AssessChain(scooperBot);
        }

        /*public void AssessChain(Entity scooperBot)
        {
            // Set scooper's nearest water source
            /*if(scooper.targetWater==null || scooper.targetWater.volume<=0){
                scooper.targetWater = scooper.FindNearestWater();
                scooper.location_PICKUP = scooper.location_DROPOFF = scooper.targetWater.transform.position;
                chain_START = scooper.location_PICKUP;
            }#1#
        }*/
    }
    
    public struct BucketChain : IComponentData
    {
        public Entity Scooper;
        public Entity Thrower;
    }
    
    public struct PasserEmpty : IBufferElementData
    {
        public Entity Value;
    }

    public struct PasserFull : IBufferElementData
    {
        public Entity Value;
    }
}
