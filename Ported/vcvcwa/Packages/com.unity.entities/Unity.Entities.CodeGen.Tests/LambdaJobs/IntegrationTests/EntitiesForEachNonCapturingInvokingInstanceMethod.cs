using System;
using NUnit.Framework;
using Unity.Entities.CodeGen.Tests.LambdaJobs.Infrastructure;
using Unity.Jobs;

namespace Unity.Entities.CodeGen.Tests
{
    [TestFixture]
    public class EntitiesForEachNonCapturingInvokingInstanceMethod : LambdaJobIntegrationTest
    {
        [Test]
        public void EntitiesForEachNonCapturingInvokingInstanceMethodTest() => RunTest(typeof(System));
        
        class System : TestJobComponentSystem
        {
            public void Test()
            {
                Entities.WithoutBurst().ForEach((ref Translation translation) => { MyInstanceMethod(123); }).Run();
            }

            public void MyInstanceMethod(int count)
            {
                Console.Write("Hello");
            }
        }
    }
}