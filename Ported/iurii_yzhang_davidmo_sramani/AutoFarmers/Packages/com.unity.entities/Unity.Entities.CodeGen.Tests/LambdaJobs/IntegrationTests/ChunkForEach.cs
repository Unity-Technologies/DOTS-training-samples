#if ENABLE_DOTS_COMPILER_CHUNKS
using NUnit.Framework;
using Unity.Entities.CodeGen.Tests.LambdaJobs.Infrastructure;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace Unity.Entities.CodeGen.Tests
{
    [TestFixture]
    public class ChunkForEach : IntegrationTest
    {
        [Test]
        public void ChunkForEachTest() => RunTest<System>();
        
        class System : JobComponentSystem
        {
            protected override JobHandle OnUpdate(JobHandle inputDeps)
            {
                int capture_from_system = 123;
                return Chunks.WithAny<Velocity, Translation>().ForEach(
                        (ArchetypeChunk chunk, int chunkIndex, int entityIndex) =>
                        {
                            int capture_from_chunklambda = 8;
                            chunk.Entities.ForEach((ref Translation t) => t.Value = capture_from_system + capture_from_chunklambda);

                            //bool callMeMaybe = true;
                            ///if (callMeMayb)
                            chunk.Entities.ForEach((in Translation t, ref Velocity v) => v.Value *= capture_from_system);
                        })
                    .Schedule(inputDeps);
            }
        }
    }
}
#endif