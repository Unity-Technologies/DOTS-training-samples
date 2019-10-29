using System;
using NUnit.Framework;
using Unity.Entities.CodeGen.Tests.LambdaJobs.Infrastructure;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Entities.CodeGen.Tests
{
    [TestFixture]
    public class SingleJob : LambdaJobIntegrationTest
    {
        [Test]
        public void SingleJobTest() => RunTest(typeof(System));
        
        class System : JobComponentSystem
        {
            protected override JobHandle OnUpdate(JobHandle inputDeps)
            {
                var myCapturedFloats = new NativeArray<float>();
                
                //make sure we're testing a Job.Run() codepath
                Job.WithName("AJobForRunning").WithCode(() => { myCapturedFloats[0] += 1; }).Run();
                
                return Job.WithName("AJobForScheduling").WithCode(() =>
                    {
                        for (int i = 0; i != myCapturedFloats.Length; i++)
                            myCapturedFloats[i] *= 2;

                    }).Schedule(inputDeps);
            }
        }
    }
}