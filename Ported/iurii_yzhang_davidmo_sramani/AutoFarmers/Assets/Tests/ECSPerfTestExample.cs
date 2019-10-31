using System.Collections;
using System.Collections.Generic;
using GameAI;
using NUnit.Framework;
using Unity.Entities;
using Unity.Entities.PerformanceTests;
using Unity.Mathematics;
using Unity.PerformanceTesting;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace PerfTests
{
    [TestFixture]
    [Category("Performance")]
    public sealed class ECSPerfTestExample : EntityPerformanceTestFixture
    {
        [Test, Performance]
        public void WorldCreatorMeasure([Values(10, 100)] int x, [Values(10, 100)] int y)
        {
            var creator = m_World.GetOrCreateSystem<WorldCreatorSystem>();
            creator.WorldSize = new int2(x, y);
            
            Measure.Method(() =>
            {
                //Assert.IsTrue(creator.Enabled);
                //Assert.IsTrue(creator.ShouldRunSystem());
                creator.Update();    
            })
                .SetUp(() =>
                {
                    var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                    var entities = entityManager.GetAllEntities();
                    entityManager.DestroyEntity(entities);
                    entities.Dispose();

                    WorldCreatorSystem.ResetExecuteOnceTag(m_Manager);
                    RenderingMapInit.ResetExecuteOnceTag(m_Manager);
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .Run();
        }

        [Test, Performance]
        public void WorldMapInitMeasure([Values(10, 100)] int x, [Values(10, 100)] int y)
        {
            var creator = m_World.GetOrCreateSystem<WorldCreatorSystem>();
            creator.WorldSize = new int2(x, y);
            
            var barrier = m_World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            var mapInit = m_World.GetOrCreateSystem<RenderingMapInit>();

            Measure.Method(() =>
                {
                    mapInit.Update();
                    barrier.Update(); // Playback happens in the update of the system that owns the command buffer.
                    m_Manager.CompleteAllJobs();
                    
                })
                .SetUp(() =>
                {
                    var go = new GameObject();
                    go.AddComponent<RenderingUnity>().Initialize();

                    m_Manager.CompleteAllJobs();
                    
                    // kill all entities
                    var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                    var entities = entityManager.GetAllEntities();
                    entityManager.DestroyEntity(entities);
                    entities.Dispose();

                    // recreate entities
                    var ru = RenderingUnity.instance;
                    ru.Initialize();

                    mapInit.CreatePrefabEntity();

                    WorldCreatorSystem.ResetExecuteOnceTag(m_Manager);
                    creator.Update();

                    RenderingMapInit.ResetExecuteOnceTag(m_Manager);
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .Run();
        }
    }
}