#if !UNITY_DISABLE_MANAGED_COMPONENTS
using System;
using NUnit.Framework;
using Unity.Entities.CodeGen.Tests.LambdaJobs.Infrastructure;
using Unity.Jobs;

namespace Unity.Entities.CodeGen.Tests
{
    [TestFixture]
    public class EntitiesForEachInForLoop : LambdaJobIntegrationTest
    {
        [Test]
        public void EntitiesForEachInForLoopTest() => RunTest(typeof(System));

        class System : JobComponentSystem
        {
            protected override JobHandle OnUpdate(JobHandle inputDeps)
            {
                int captureMe = 3;

                for (int i = 0; i != 3; i++)
                {
                    Entities
                        .WithoutBurst()
                        .ForEach((ref Translation translation, in ManagedComponent m) => translation.Value += captureMe)
                        .Run();
                }

                return default;
            }
        }
    }
    
    class ManagedComponent : IComponentData, IEquatable<ManagedComponent>
    {
        public bool Equals(ManagedComponent other) => false;
        public override bool Equals(object obj) => false;
        public override int GetHashCode() =>  0;
    }
}
#endif