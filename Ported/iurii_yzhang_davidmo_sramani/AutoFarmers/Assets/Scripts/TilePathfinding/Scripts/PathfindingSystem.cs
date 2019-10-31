using System;
using GameAI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;


namespace Pathfinding
{
    public class PathfindingSystem : JobComponentSystem
    {
        BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;
        private DistanceField m_distanceFieldPlant;
        private DistanceField m_distanceFieldStone;
        private DistanceField m_distanceFieldShop;
        private int2 m_worldSize;

        protected override void OnCreate()
        {
            m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            var plantQuery = EntityManager.CreateEntityQuery(typeof(PlantPositionRequest));
            var stoneQuery = EntityManager.CreateEntityQuery(typeof(StonePositionRequest));
            var shopQuery = EntityManager.CreateEntityQuery(typeof(ShopPositionRequest));
            m_worldSize = World.GetOrCreateSystem<WorldCreatorSystem>().WorldSize;
            
            m_distanceFieldShop = new DistanceField(DistanceField.FieldType.Shop, m_worldSize, plantQuery, stoneQuery, shopQuery);
            m_distanceFieldPlant = new DistanceField(DistanceField.FieldType.Plant, m_worldSize, plantQuery, stoneQuery, shopQuery);
            m_distanceFieldStone = new DistanceField(DistanceField.FieldType.Stone, m_worldSize, plantQuery, stoneQuery, shopQuery);
            m_distanceFieldPlant.Schedule();
            m_distanceFieldStone.Schedule();
            m_distanceFieldShop.Schedule();
        }

        protected override void OnDestroy()
        {
            m_distanceFieldPlant.Dispose();
            m_distanceFieldStone.Dispose();
            m_distanceFieldShop.Dispose();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var worldSize = m_worldSize;

            // find Untilled Tile target
            var ecb2 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job2 = Entities
                .WithAll<AISubTaskTagFindUntilledTile>()
                .WithNone<AISubTaskTagComplete>()
                .WithNone<HasTarget>()
                //.WithoutBurst()
                .ForEach((Entity entity, int nativeThreadIndex, in TilePositionable pos) =>
                {
                    // Distance Field will provide the target position
                    // Add HasTarget.TargetPosition
                    int2 targetpos = new int2(10, 10);
                    ecb2.AddComponent(nativeThreadIndex, entity, new HasTarget(targetpos));
                }).Schedule(inputDeps);

            // reach the tilling target
            var ecb3 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job3 = Entities
                .WithAll<AISubTaskTagFindUntilledTile>()
                .WithNone<AISubTaskTagComplete>()
                //.WithoutBurst()
                .ForEach((Entity entity, int nativeThreadIndex, in TilePositionable tile, in HasTarget t) =>
                {
                    bool2 tt = (t.TargetPosition == tile.Position);
                    if (tt[0] && tt[1])
                    {
                        // Add tag: subTaskComplete
                        ecb3.AddComponent<AISubTaskTagComplete>(nativeThreadIndex, entity);
                    }
                }).Schedule(inputDeps);

            // till current tile
            var ecb4 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job4 = Entities
                .WithAll<AISubTaskTagTillGroundTile>()
                .WithNone<AISubTaskTagComplete>()
                //.WithoutBurst()
                .ForEach((Entity entity, int nativeThreadIndex) =>
                {
                    ecb4.AddComponent<AISubTaskTagComplete>(nativeThreadIndex, entity);
                }).Schedule(inputDeps);

            // Find Rock
            var ecb5 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            m_distanceFieldStone.Complete();
            var distanceFieldStoneRead = m_distanceFieldStone.DistFieldRead;
            var job5 = Entities
                .WithAll<AISubTaskTagFindRock>()
                .WithNone<AISubTaskTagComplete>()
                .WithNone<HasTarget>()
                //.WithoutBurst()
                .ForEach((Entity entity, int entityInQueryIndex, in TilePositionable tile) =>
                {
                    bool reached = false;
                    int2 target = DistanceField.PathTo(tile.Position, worldSize, distanceFieldStoneRead, out reached);
//                    int value = DistanceField.GetDistanceFieldValue(tile.Position, worldSize, distanceFieldStoneRead);
//                    Debug.Log($"Path2 : {tile.Position} -> {target} = {reached}. dist field val = {value}");
                    if (reached) {
                        ecb5.AddComponent(entityInQueryIndex, entity, new AISubTaskTagComplete() {targetPos = target});
                    } else {
                        ecb5.AddComponent(entityInQueryIndex, entity, new HasTarget(target));
                    }
                }).Schedule(inputDeps);

            // smash rock 
            var ecb6 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job6 = Entities
                .WithAll<AISubTaskTagTillGroundTile>()
                .WithNone<AISubTaskTagComplete>()
                //.WithoutBurst()
                .ForEach((Entity entity, int nativeThreadIndex, in TilePositionable tile, in HasTarget t) =>
                {
                    bool2 tt = (t.TargetPosition == tile.Position);
                    if (tt[0] && tt[1])
                    {
                        // Add tag: subTaskComplete
                        ecb6.AddComponent<AISubTaskTagComplete>(nativeThreadIndex, entity);
                    }
                }).Schedule(inputDeps);

            // Seeding
            var ecb7 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job7 = Entities
                .WithAll<AISubTaskTagPlantSeed>()
                .WithNone<AISubTaskTagComplete>()
                //.WithoutBurst()
                .ForEach((Entity entity, int nativeThreadIndex) =>
                {
                    // Add tag: subTaskComplete
                    ecb7.AddComponent<AISubTaskTagComplete>(nativeThreadIndex, entity);
                }).Schedule(inputDeps);

            // Find Plant
            var ecb8 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            m_distanceFieldPlant.Complete();
            var distanceFieldPlantRead = m_distanceFieldPlant.DistFieldRead;
            
            var job8 = Entities
                .WithAll<AISubTaskTagFindPlant>()
                .WithNone<AISubTaskTagComplete>()
                .WithNone<HasTarget>()
                //.WithoutBurst()
                //.WithReadOnly(distanceFieldPlantRead)  
                .ForEach((Entity entity, int entityInQueryIndex, in TilePositionable position) =>
                {
                    bool reached = false;
                    int2 target = DistanceField.PathTo(position.Position, worldSize, distanceFieldPlantRead, out reached);
                    if (reached) {
                        ecb8.AddComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    } else {
                        ecb8.AddComponent(entityInQueryIndex, entity, new HasTarget(target));
                    }
                }).Schedule(inputDeps);
            
            // Find shop
            m_distanceFieldShop.Complete();
            var distanceFieldShopRead = m_distanceFieldShop.DistFieldRead;
            
            var ecb9 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job9 = Entities
                .WithAll<AISubTaskTagFindShop>()
                .WithNone<AISubTaskTagComplete>()
                .WithNone<HasTarget>()
                //.WithoutBurst()
                .ForEach((Entity entity, int entityInQueryIndex, in TilePositionable position) =>
                {
                    bool reached = false;
                    int2 target = DistanceField.PathTo(position.Position, worldSize, distanceFieldShopRead, out reached);
                    if (reached) {
                        ecb9.AddComponent<AISubTaskTagComplete>(entityInQueryIndex, entity);
                    } else {
                        ecb9.AddComponent(entityInQueryIndex, entity, new HasTarget(target));
                    }
                }).Schedule(inputDeps);

            // Sell Plant
            var ecb10 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job10 = Entities
                .WithAll<AISubTaskTagFindShop>()
                .WithNone<AISubTaskTagComplete>()
                //.WithoutBurst()
                .ForEach((Entity entity, int nativeThreadIndex, in TilePositionable tile, in HasTarget t) =>
                {
                    bool2 tt = (t.TargetPosition == tile.Position);
                    if (tt[0] && tt[1])
                    {
                        // Add tag: subTaskComplete
                        ecb10.AddComponent<AISubTaskTagComplete>(nativeThreadIndex, entity);
                    }
                }).Schedule(inputDeps);

            // m_EntityCommandBufferSystem.AddJobHandleForProducer(job1);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job3);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job4);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job5);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job6);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job7);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job8);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job9);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job10);

            return JobHandle.CombineDependencies(
                    JobHandle.CombineDependencies(job10, job2, job3),
                    JobHandle.CombineDependencies(job4, job5, job6),
                    JobHandle.CombineDependencies(job7, job8, job9));
        }

        public void PlantOrStoneChanged()
        {
            m_distanceFieldShop.Complete();
            m_distanceFieldPlant.Complete();
            m_distanceFieldStone.Complete();
            
            m_distanceFieldShop.Schedule();
            m_distanceFieldPlant.Schedule();
            m_distanceFieldStone.Schedule();
        }
    }
}
