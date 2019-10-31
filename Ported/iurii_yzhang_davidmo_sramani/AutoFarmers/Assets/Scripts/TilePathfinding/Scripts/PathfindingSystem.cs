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
using Random = System.Random;


namespace AutoFarmersTests
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class PathfindingSystem : JobComponentSystem
    {
        BeginInitializationEntityCommandBufferSystem m_EntityCommandBufferSystem;

        protected override void OnCreate()
        {
            m_EntityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var worldSize = World.GetOrCreateSystem<WorldCreatorSystem>().WorldSize;

            var ecb1 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job1 = Entities
                .WithAll<FarmerComponent>()
                .ForEach((Entity entity, int nativeThreadIndex, ref TilePositionable positionable) =>
                {
                    var dirX = new int4(1, -1, 0, 0);
                    var dirY = new int4(0, 0, 1, -1);

                    Random r = new Random();
                    int rInt = r.Next(0, 4);

                    int x2 = positionable.Position[0] + dirX[rInt];
                    int y2 = positionable.Position[1] + dirY[rInt];

                    if (x2 < 0 || x2 >= worldSize.x)
                    {
                        x2 = positionable.Position[0] + dirX[(rInt + 1) % 3];
                    }

                    if (y2 < 0 || y2 >= worldSize.y)
                    {
                        y2 = positionable.Position[1] + dirX[(rInt + 1) % 3];
                    }

                    positionable.Position = new int2(x2, y2);
                }).Schedule(inputDeps);

            // find Untilled Tile target
            var ecb2 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job2 = Entities
                .WithAll<AISubTaskTagFindUntilledTile>()
                .WithNone<AISubTaskTagComplete>()
                .WithNone<HasTarget>()
                .ForEach((Entity entity, int nativeThreadIndex, ref TilePositionable pos) =>
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
                .ForEach((Entity entity, int nativeThreadIndex, ref TilePositionable tile, ref HasTarget t) =>
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
                .ForEach((Entity entity, int nativeThreadIndex, ref TilePositionable tile) =>
                {
                    ecb4.AddComponent<AISubTaskTagComplete>(nativeThreadIndex, entity);
                }).Schedule(inputDeps);

            // Find Rock
            var ecb5 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job5 = Entities
                .WithAll<AISubTaskTagFindRock>()
                .WithNone<AISubTaskTagComplete>()
                .WithNone<HasTarget>()
                .ForEach((Entity entity, int nativeThreadIndex, ref TilePositionable tile) =>
                {
                    // Distance Field will provide the target position
                    // Add HasTarget.TargetPosition
                    int2 targetpos = new int2(10, 10);
                    ecb5.AddComponent(nativeThreadIndex, entity, new HasTarget(targetpos));
                }).Schedule(inputDeps);

            // smash rock 
            var ecb6 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job6 = Entities
                .WithAll<AISubTaskTagTillGroundTile>()
                .WithNone<AISubTaskTagComplete>()
                .ForEach((Entity entity, int nativeThreadIndex, ref TilePositionable tile, ref HasTarget t) =>
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
                .ForEach((Entity entity, int nativeThreadIndex, ref TilePositionable tile) =>
                {
                    // Add tag: subTaskComplete
                    ecb7.AddComponent<AISubTaskTagComplete>(nativeThreadIndex, entity);
                }).Schedule(inputDeps);

            // Find Plant
            var ecb8 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job8 = Entities
                .WithAll<AISubTaskTagFindPlant>()
                .WithNone<AISubTaskTagComplete>()
                .WithNone<HasTarget>()
                .ForEach((Entity entity, int nativeThreadIndex, ref TilePositionable tile) =>
                {
                    // Distance Field will provide the target position
                    // Add HasTarget.TargetPosition
                    int2 targetpos = new int2(20, 20);
                    ecb8.AddComponent(nativeThreadIndex, entity, new HasTarget(targetpos));
                }).Schedule(inputDeps);

            // Find shop
            var ecb9 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job9 = Entities
                .WithAll<AISubTaskTagFindShop>()
                .WithNone<AISubTaskTagComplete>()
                .WithNone<HasTarget>()
                .ForEach((Entity entity, int nativeThreadIndex, ref TilePositionable tile) =>
                {
                    // Distance Field will provide the target position
                    // Add HasTarget.TargetPosition
                    int2 targetpos = new int2(30, 30);
                    ecb9.AddComponent(nativeThreadIndex, entity, new HasTarget(targetpos));
                }).Schedule(inputDeps);

            // Sell Plant
            var ecb10 = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var job10 = Entities
                .WithAll<AISubTaskTagFindShop>()
                .WithNone<AISubTaskTagComplete>()
                .ForEach((Entity entity, int nativeThreadIndex, ref TilePositionable tile, ref HasTarget t) =>
                {
                    bool2 tt = (t.TargetPosition == tile.Position);
                    if (tt[0] && tt[1])
                    {
                        // Add tag: subTaskComplete
                        ecb10.AddComponent<AISubTaskTagComplete>(nativeThreadIndex, entity);
                    }
                }).Schedule(inputDeps);

            m_EntityCommandBufferSystem.AddJobHandleForProducer(job1);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job2);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job3);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job4);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job5);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job6);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job7);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job8);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job9);
            m_EntityCommandBufferSystem.AddJobHandleForProducer(job10);

            NativeArray<JobHandle> x = new NativeArray<JobHandle>(10, Allocator.TempJob);
            var jarray = new JobHandle[] {job1, job2, job3, job4, job5, job6, job7, job8, job9, job10};
            x.CopyFrom(jarray);
            
            var handle = JobHandle.CombineDependencies(x);

            return handle;
        }
    }
}
