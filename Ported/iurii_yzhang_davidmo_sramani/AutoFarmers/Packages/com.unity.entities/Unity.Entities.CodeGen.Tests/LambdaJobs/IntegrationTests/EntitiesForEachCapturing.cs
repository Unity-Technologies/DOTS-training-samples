using System;
using NUnit.Framework;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities.CodeGen.Tests.LambdaJobs.Infrastructure;
using Unity.Entities.CodeGeneratedJobForEach;
using Unity.Jobs;

namespace Unity.Entities.CodeGen.Tests
{
    [TestFixture]
    public class EntitiesForEachCapturing : LambdaJobIntegrationTest
    {
        [Test]
        public void EntitiesForEachCapturingTest() => RunTest(typeof(System));
        
        class System : JobComponentSystem
        {
            protected override unsafe JobHandle OnUpdate(JobHandle inputDeps)
            {
                var myCapturedFloats = new NativeArray<float>();
                byte* myRawPtr = (byte*)IntPtr.Zero;
                
                return Entities
                    .WithBurst(FloatMode.Deterministic, FloatPrecision.High,synchronousCompilation: true)
                    .WithEntityQueryOptions(EntityQueryOptions.IncludeDisabled)
                    .WithChangeFilter<Translation>()
                    .WithNone<Boid>()
                    .WithAll<Velocity>()
                    .WithReadOnly(myCapturedFloats)
                    .WithDeallocateOnJobCompletion(myCapturedFloats)
                    .WithNativeDisableContainerSafetyRestriction(myCapturedFloats)
                    .WithNativeDisableUnsafePtrRestriction(myRawPtr)
                    .ForEach(
                        (int entityInQueryIndex,
                            Entity myEntity,
                            DynamicBuffer<MyBufferInt> myBufferInts, ref Translation translation, in Acceleration acceleration) =>
                        {
                            translation.Value += (myCapturedFloats[2] + acceleration.Value + entityInQueryIndex + myEntity.Version + myBufferInts[2].Value);
                            Console.Write(myRawPtr->ToString());
                        })
                    .Schedule(inputDeps);
            }
        }
    }
}