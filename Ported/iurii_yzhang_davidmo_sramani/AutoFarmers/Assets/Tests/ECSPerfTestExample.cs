using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Entities;
using Unity.Entities.PerformanceTests;
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
        public void DemoPerfTest([Values(1, 10, 100)] int n, [Values(100, 1000)] int entityCount)
        {
            var atype = m_Manager.CreateArchetype(typeof(Translation), typeof(LocalToWorld), typeof(RenderMesh));

            Measure.Method(() =>
                {
                    for (int i = 0; i < n; ++i)
                    {
                        m_Manager.CreateEntity(atype);
                    }
                })
                .Definition("DemoTest")
                .WarmupCount(100)
                .MeasurementCount(500)
                .Run();
        }
    }
}