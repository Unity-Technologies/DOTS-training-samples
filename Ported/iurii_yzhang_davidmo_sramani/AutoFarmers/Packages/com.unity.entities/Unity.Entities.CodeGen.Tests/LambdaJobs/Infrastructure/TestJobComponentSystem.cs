using Unity.Entities;
using Unity.Jobs;

[assembly: DisableAutoCreation]

namespace Unity.Entities.CodeGen.Tests.LambdaJobs.Infrastructure
{
    public class TestJobComponentSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps) => default;
    }
}