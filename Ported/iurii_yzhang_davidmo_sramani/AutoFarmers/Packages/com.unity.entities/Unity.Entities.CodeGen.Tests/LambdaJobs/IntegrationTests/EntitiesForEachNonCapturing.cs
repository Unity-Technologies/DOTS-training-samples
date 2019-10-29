using NUnit.Framework;
using Unity.Entities.CodeGen.Tests.LambdaJobs.Infrastructure;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Entities.CodeGen.Tests
{
    [TestFixture]
    public class EntitiesForEachNonCapturing : LambdaJobIntegrationTest
    {
        [Test]
        public void EntitiesForEachNonCapturingTest() => RunTest(typeof(System));
        
        class System : JobComponentSystem
        {
            protected override JobHandle OnUpdate(JobHandle inputDeps)
            {
                Entities.ForEach((ref Translation translation) => { translation.Value += 5; }).Run();
                return default;
            }
        }
    }
}