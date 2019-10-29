using NUnit.Framework;
using Unity.Collections;
using Unity.Entities.CodeGen.Tests.LambdaJobs.Infrastructure;
using Unity.Jobs;

namespace Unity.Entities.CodeGen.Tests
{
    [TestFixture]
    public class DeallocateOnCompletion_Run : LambdaJobIntegrationTest
    {
        [Test]
        public void DeallocateOnCompletion_RunTest() => RunTest(typeof(System));
        
        class System : JobComponentSystem
        {
            protected override JobHandle OnUpdate(JobHandle inputDeps)
            {
                var myNativeArray = new NativeArray<float>();
                var myList = new NativeList<int>();
                var hashmap = new NativeHashMap<int,int>();
                
                Entities
                    .WithDeallocateOnJobCompletion(myNativeArray)
                    .WithDeallocateOnJobCompletion(myList)
                    .WithDeallocateOnJobCompletion(hashmap)
                    .ForEach((Entity e) =>
                    {
                        myNativeArray[1]++;
                        myList[0]++;
                        hashmap[3] = 4;
                    })
                    .Run();

                return default;
            }
        }
    }
}