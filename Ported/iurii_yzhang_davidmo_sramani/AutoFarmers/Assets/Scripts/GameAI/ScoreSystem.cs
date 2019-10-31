using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

namespace GameAI
{
    public class ScoreSystem : JobComponentSystem
    {
        public int Score;
        BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

        protected override void OnCreate()
        {
            m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            // World.GetOrCreateSystem<WorldCreatorSystem>().
            Score = 0;
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            // sell plant +1
            var ecb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            // how to know the plant has been sold?
            // the sub task sell plant 
            var job = Entities
                .WithAll<AISubTaskSellPlant>()
                .WithNone<AISubTaskTagComplete>()
                .ForEach((Entity entity, int entityInQueryIndex) => 
                {
                    ecb.AddComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    //Score++;
                }).Schedule(inputDeps);

            // smash rock + 1
            var ecb2 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job2 = Entities
                .WithAll<AISubTaskTagClearRock>()
                .WithNone<AISubTaskTagComplete>()
                .ForEach((Entity entity, int entityInQueryIndex) => 
                {
                    ecb2.AddComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    //Score++;
                }).Schedule(inputDeps);

            // spawn farmer -15
            // add SpawnFarmerTagComponent
            // add SpawnPointComponent
            var ecb3 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job3 = Entities
                .WithAll<TagShop>()
                .WithoutBurst()
                .ForEach((Entity e, int entityInQueryIndex, in TilePositionable tile) =>
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
                }).Schedule(inputDeps);
            
            // spawn drone -20
            // add SpawnDroneTagComponent
            // add SpawnPointComponent
            var ecb4 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job4 = Entities
                .WithAll<TagShop>()
                .WithoutBurst()
                .ForEach((Entity e, int entityInQueryIndex, in TilePositionable tile) =>
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
                }).Schedule(inputDeps);

            m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job3);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job4);

            return JobHandle.CombineDependencies(JobHandle.CombineDependencies(job, job2), JobHandle.CombineDependencies(job3, job4));
        }
    }
}